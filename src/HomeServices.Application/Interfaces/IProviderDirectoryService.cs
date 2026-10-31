using HomeServices.Application.DTOs.Providers;

namespace HomeServices.Application.Interfaces;

/// <summary>
/// استعلامات القراءة العامة على مقدمي الخدمات الموثّقين (العرض والبحث).
/// </summary>
public interface IProviderDirectoryService
{
    Task<IReadOnlyList<ProviderCardDto>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default);
}
