using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Files;
using HomeServices.Application.DTOs.ProviderManagement;

namespace HomeServices.Application.Interfaces;

/// <summary>
/// إدارة مقدم الخدمة لملفه الخاص. كل العمليات مقيّدة بملف المستخدم الحالي (userId).
/// </summary>
public interface IProviderProfileService
{
    public const int MaxServices = 30;
    public const int MaxAreas = 40;
    public const int MaxPortfolioImages = 12;

    Task<ProviderDashboardDto?> GetDashboardAsync(string userId, CancellationToken cancellationToken = default);

    Task<ProviderNavInfo?> GetNavInfoAsync(string userId, CancellationToken cancellationToken = default);

    Task<ProviderProfileEditDto?> GetProfileForEditAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result> UpdateProfileAsync(string userId, ProviderProfileEditDto dto, CancellationToken cancellationToken = default);

    Task<Result> SetAvailabilityAsync(string userId, bool isAvailable, CancellationToken cancellationToken = default);

    /// <summary>
    /// يعيد إرسال ملف مرفوض للمراجعة بعد تعديله، بشرط اكتمال البيانات المطلوبة.
    /// </summary>
    Task<Result> ResubmitForReviewAsync(string userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ManagedServiceDto>> GetServicesAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// يضيف الخدمة أو يحدّث سعرها إن كانت مضافة مسبقاً.
    /// </summary>
    Task<Result> SaveServiceAsync(string userId, ProviderServiceInput input, CancellationToken cancellationToken = default);

    Task<Result> RemoveServiceAsync(string userId, int providerServiceId, CancellationToken cancellationToken = default);

    Task<IReadOnlySet<int>> GetAreaIdsAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result> UpdateAreasAsync(string userId, IReadOnlyCollection<int> areaIds, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ManagedPortfolioImageDto>> GetPortfolioAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result> AddPortfolioImageAsync(string userId, FileUpload image, string? caption, CancellationToken cancellationToken = default);

    Task<Result> RemovePortfolioImageAsync(string userId, int imageId, CancellationToken cancellationToken = default);
}
