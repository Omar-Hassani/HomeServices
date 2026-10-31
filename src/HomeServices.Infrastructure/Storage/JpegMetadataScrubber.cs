using System.Buffers.Binary;
using System.Text;

namespace HomeServices.Infrastructure.Storage;

/// <summary>
/// يزيل بيانات الموقع الجغرافي من صور JPEG قبل حفظها، دون مكتبات خارجية:
/// - يمسح محتوى GPS IFD داخل EXIF (الإحداثيات وكل قيمها) ويُبقي بقية الوسوم مثل Orientation حتى لا تنقلب الصورة.
/// - يحذف مقطع XMP بالكامل لأنه قد يحتوي الموقع أيضاً ولا يلزم للعرض.
/// عند أي بنية غير متوقعة تُعاد الصورة كما هي (لا نُفسد صورة صالحة).
/// </summary>
internal static class JpegMetadataScrubber
{
    private const byte Marker = 0xFF;
    private const byte StartOfScan = 0xDA;
    private const byte App1 = 0xE1;
    private const ushort GpsIfdTag = 0x8825;

    private static readonly byte[] ExifHeader = "Exif\0\0"u8.ToArray();
    private static readonly byte[] XmpHeader = Encoding.ASCII.GetBytes("http://ns.adobe.com/xap/1.0/");

    public static byte[] Scrub(byte[] jpeg)
    {
        if (jpeg.Length < 4 || jpeg[0] != Marker || jpeg[1] != 0xD8)
        {
            return jpeg;
        }

        try
        {
            using var output = new MemoryStream(jpeg.Length);
            output.Write(jpeg, 0, 2);
            var position = 2;

            while (position + 4 <= jpeg.Length)
            {
                if (jpeg[position] != Marker)
                {
                    return jpeg;
                }

                var type = jpeg[position + 1];
                if (type == StartOfScan)
                {
                    // بعد بداية بيانات الصورة لا توجد بيانات وصفية: ننسخ الباقي كما هو
                    output.Write(jpeg, position, jpeg.Length - position);
                    return output.ToArray();
                }

                var length = BinaryPrimitives.ReadUInt16BigEndian(jpeg.AsSpan(position + 2));
                var segmentEnd = position + 2 + length;
                if (length < 2 || segmentEnd > jpeg.Length)
                {
                    return jpeg;
                }

                var dataStart = position + 4;
                var data = jpeg.AsSpan(dataStart, length - 2);

                if (type == App1 && data.StartsWith(XmpHeader))
                {
                    position = segmentEnd;
                    continue;
                }

                if (type == App1 && data.StartsWith(ExifHeader))
                {
                    ClearGps(jpeg, dataStart + ExifHeader.Length, segmentEnd);
                }

                output.Write(jpeg, position, segmentEnd - position);
                position = segmentEnd;
            }

            return jpeg;
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException or IndexOutOfRangeException)
        {
            return jpeg;
        }
    }

    /// <summary>
    /// يمسح كل مدخلات GPS IFD والقيم التي تشير إليها، ثم يجعل عدد المدخلات صفراً.
    /// </summary>
    private static void ClearGps(byte[] buffer, int tiffStart, int segmentEnd)
    {
        if (tiffStart + 8 > segmentEnd)
        {
            return;
        }

        var littleEndian = buffer[tiffStart] == (byte)'I';
        ushort ReadUInt16(int offset) => littleEndian
            ? BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(offset))
            : BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan(offset));
        uint ReadUInt32(int offset) => littleEndian
            ? BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(offset))
            : BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan(offset));

        var ifd0 = tiffStart + (int)ReadUInt32(tiffStart + 4);
        if (ifd0 + 2 > segmentEnd)
        {
            return;
        }

        int? gpsIfd = null;
        var entries = ReadUInt16(ifd0);
        for (var i = 0; i < entries; i++)
        {
            var entry = ifd0 + 2 + i * 12;
            if (entry + 12 > segmentEnd)
            {
                return;
            }

            if (ReadUInt16(entry) == GpsIfdTag)
            {
                gpsIfd = tiffStart + (int)ReadUInt32(entry + 8);
                break;
            }
        }

        if (gpsIfd is not int gps || gps + 2 > segmentEnd)
        {
            return;
        }

        var gpsEntries = ReadUInt16(gps);
        for (var i = 0; i < gpsEntries; i++)
        {
            var entry = gps + 2 + i * 12;
            if (entry + 12 > segmentEnd)
            {
                break;
            }

            var valueSize = TypeSize(ReadUInt16(entry + 2)) * (long)ReadUInt32(entry + 4);
            if (valueSize > 4)
            {
                var valueStart = tiffStart + (long)ReadUInt32(entry + 8);
                if (valueStart >= tiffStart && valueStart + valueSize <= segmentEnd)
                {
                    Array.Clear(buffer, (int)valueStart, (int)valueSize);
                }
            }

            Array.Clear(buffer, entry, 12);
        }

        Array.Clear(buffer, gps, 2);
    }

    private static int TypeSize(ushort type) => type switch
    {
        1 or 2 or 6 or 7 => 1,
        3 or 8 => 2,
        4 or 9 or 11 => 4,
        5 or 10 or 12 => 8,
        _ => 1
    };
}
