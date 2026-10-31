using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Files;
using HomeServices.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace HomeServices.Infrastructure.Storage;

/// <summary>
/// يحفظ الملفات في wwwroot/uploads/{folder}/{yyyy}/{MM}/{guid}.{ext}.
/// نوع الصورة يُحدَّد من محتوى الملف (التوقيع) وليس من امتداده أو من نوعه المُرسل من المتصفح.
/// </summary>
internal sealed class LocalFileStorageService(IWebHostEnvironment environment, ILogger<LocalFileStorageService> logger) : IFileStorageService
{
    private const string UploadsFolder = "uploads";

    public async Task<Result<string>> SaveImageAsync(FileUpload file, string folder, CancellationToken cancellationToken = default)
    {
        if (file.Length <= 0)
        {
            return Result<string>.Failure("الملف المرفوع فارغ.");
        }

        if (file.Length > IFileStorageService.MaxImageBytes)
        {
            return Result<string>.Failure("حجم الصورة يجب ألا يتجاوز 5 ميغابايت.");
        }

        using var buffer = new MemoryStream();
        await file.Content.CopyToAsync(buffer, cancellationToken);

        var extension = DetectImageExtension(buffer.GetBuffer().AsSpan(0, (int)Math.Min(buffer.Length, 16)));
        if (extension is null)
        {
            return Result<string>.Failure("صيغة الملف غير مدعومة. يُسمح بصور JPG وPNG وWebP فقط.");
        }

        var now = DateTime.UtcNow;
        var relativeDirectory = Path.Combine(UploadsFolder, SanitizeFolder(folder), now.ToString("yyyy"), now.ToString("MM"));
        var absoluteDirectory = Path.Combine(WebRoot, relativeDirectory);
        Directory.CreateDirectory(absoluteDirectory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(absoluteDirectory, fileName);

        buffer.Position = 0;
        await using (var output = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write))
        {
            await buffer.CopyToAsync(output, cancellationToken);
        }

        var publicPath = "/" + Path.Combine(relativeDirectory, fileName).Replace('\\', '/');
        return Result<string>.Success(publicPath);
    }

    public Task DeleteAsync(string? path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path) || !path.StartsWith($"/{UploadsFolder}/", StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        // منع الخروج من مجلد الرفع عبر مسارات مثل ../
        var uploadsRoot = Path.GetFullPath(Path.Combine(WebRoot, UploadsFolder)) + Path.DirectorySeparatorChar;
        var absolutePath = Path.GetFullPath(Path.Combine(WebRoot, path.TrimStart('/')));
        if (!absolutePath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Refused to delete a file outside the uploads folder: {Path}", path);
            return Task.CompletedTask;
        }

        try
        {
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }
        }
        catch (IOException ex)
        {
            // فشل حذف ملف قديم لا يجب أن يُفشل العملية الأساسية
            logger.LogWarning(ex, "Could not delete uploaded file {Path}", path);
        }

        return Task.CompletedTask;
    }

    private string WebRoot => environment.WebRootPath
        ?? throw new InvalidOperationException("WebRootPath is not configured.");

    private static string SanitizeFolder(string folder) =>
        new(folder.Where(c => char.IsAsciiLetterOrDigit(c) || c == '-').ToArray());

    private static string? DetectImageExtension(ReadOnlySpan<byte> header)
    {
        if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return ".jpg";
        }

        if (header.Length >= 8 && header[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
        {
            return ".png";
        }

        if (header.Length >= 12 && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8))
        {
            return ".webp";
        }

        return null;
    }
}
