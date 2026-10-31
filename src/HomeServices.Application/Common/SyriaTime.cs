namespace HomeServices.Application.Common;

/// <summary>
/// التوقيت المحلي لسوريا. التواريخ تُخزَّن بـ UTC وتُحوَّل لتوقيت دمشق عند العرض والتحقق من المواعيد.
/// </summary>
public static class SyriaTime
{
    private static readonly TimeZoneInfo Zone = ResolveZone();

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);

    public static DateOnly Today => DateOnly.FromDateTime(Now);

    public static DateTime FromUtc(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone);

    private static TimeZoneInfo ResolveZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Damascus");
        }
        catch (TimeZoneNotFoundException)
        {
            // سوريا تعتمد UTC+3 على مدار السنة
            return TimeZoneInfo.CreateCustomTimeZone("Syria", TimeSpan.FromHours(3), "Syria", "Syria");
        }
    }
}
