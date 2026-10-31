using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Files;

namespace HomeServices.Application.Interfaces;

/// <summary>
/// تخزين الملفات المرفوعة. التطبيق الحالي محلي (wwwroot/uploads) ويمكن استبداله بتخزين سحابي.
/// </summary>
public interface IFileStorageService
{
    public const long MaxImageBytes = 5 * 1024 * 1024;

    /// <summary>
    /// يتحقق من أن الملف صورة حقيقية (JPEG/PNG/WebP) وحجمه مقبول، ثم يحفظه ويُرجع مساره العام.
    /// </summary>
    Task<Result<string>> SaveImageAsync(FileUpload file, string folder, CancellationToken cancellationToken = default);

    Task DeleteAsync(string? path, CancellationToken cancellationToken = default);
}
