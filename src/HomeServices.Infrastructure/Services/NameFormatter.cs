namespace HomeServices.Infrastructure.Services;

internal static class NameFormatter
{
    /// <summary>
    /// لحماية خصوصية العملاء في التقييمات العامة: الاسم الأول والحرف الأول من الكنية فقط.
    /// </summary>
    public static string ToPrivateDisplayName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "عميل",
            1 => parts[0],
            _ => $"{parts[0]} {parts[^1][0]}."
        };
    }
}
