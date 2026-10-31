namespace HomeServices.Application.DTOs.Files;

/// <summary>
/// ملف مرفوع بصيغة مستقلة عن ASP.NET Core (IFormFile)، ليبقى منطق التخزين قابلاً للاستبدال والاختبار.
/// </summary>
public sealed record FileUpload(Stream Content, string FileName, long Length);

public static class StorageFolders
{
    public const string ProfileImages = "profiles";
    public const string Portfolio = "portfolio";
}
