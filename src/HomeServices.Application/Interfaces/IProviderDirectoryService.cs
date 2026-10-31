using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Providers;

namespace HomeServices.Application.Interfaces;

/// <summary>
/// استعلامات القراءة العامة على مقدمي الخدمات الموثّقين (العرض والبحث).
/// </summary>
public interface IProviderDirectoryService
{
    Task<IReadOnlyList<ProviderCardDto>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default);

    Task<PagedResult<ProviderCardDto>> SearchAsync(ProviderSearchCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// تفاصيل مقدم خدمة. الملفات غير الموثّقة لا تُرجع إلا لصاحبها أو للمدير (للمعاينة).
    /// </summary>
    Task<ProviderDetailsDto?> GetDetailsAsync(int providerId, string? viewerUserId, bool viewerIsAdmin, CancellationToken cancellationToken = default);
}
