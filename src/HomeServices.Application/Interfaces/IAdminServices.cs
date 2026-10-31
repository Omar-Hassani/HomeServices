using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Admin;
using HomeServices.Application.DTOs.Orders;
using HomeServices.Domain.Enums;

namespace HomeServices.Application.Interfaces;

public interface IAdminDashboardService
{
    Task<AdminStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// مراجعة ملفات مقدمي الخدمات وتوثيقها. كل قرار يُرسل إشعاراً لمقدم الخدمة.
/// </summary>
public interface IProviderVerificationService
{
    Task<PagedResult<AdminProviderRowDto>> GetProvidersAsync(VerificationStatus? status, string? query, int page, CancellationToken cancellationToken = default);

    Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default);

    Task<Result> ApproveAsync(int providerId, string? note, CancellationToken cancellationToken = default);

    Task<Result> RejectAsync(int providerId, string note, CancellationToken cancellationToken = default);

    Task<Result> SuspendAsync(int providerId, string note, CancellationToken cancellationToken = default);
}

public interface IUserAdminService
{
    Task<PagedResult<AdminUserRowDto>> GetUsersAsync(string? query, string? role, UserStatusFilter status, int page, CancellationToken cancellationToken = default);

    /// <summary>
    /// يوقف الحساب أو يفعّله. الإيقاف يغيّر SecurityStamp فتنتهي جلسات المستخدم المفتوحة.
    /// لا يمكن للمدير إيقاف حسابه أو حساب مدير آخر.
    /// </summary>
    Task<Result> SetActiveAsync(string adminUserId, string userId, bool isActive, CancellationToken cancellationToken = default);
}

public interface IAdminOrderService
{
    Task<PagedResult<AdminOrderRowDto>> GetOrdersAsync(OrderStatus? status, string? query, int page, CancellationToken cancellationToken = default);

    Task<OrderCountsDto> GetCountsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// تفاصيل أي طلب للمدير (قراءة فقط، دون إجراءات).
    /// </summary>
    Task<AdminOrderDetailsDto?> GetDetailsAsync(int orderId, CancellationToken cancellationToken = default);
}

public sealed record AdminOrderDetailsDto(
    OrderDetailsDto Order,
    string CustomerName,
    string? CustomerPhone,
    string CustomerEmail,
    string ProviderName,
    string? ProviderPhone);

public interface IReviewModerationService
{
    Task<PagedResult<AdminReviewRowDto>> GetReviewsAsync(ReviewVisibilityFilter visibility, byte? rating, int page, CancellationToken cancellationToken = default);

    /// <summary>
    /// يخفي التقييم أو يظهره، ويعيد حساب متوسط تقييم مقدم الخدمة.
    /// </summary>
    Task<Result> SetHiddenAsync(int reviewId, bool isHidden, CancellationToken cancellationToken = default);
}

public interface ICatalogAdminService
{
    Task<IReadOnlyList<AdminCategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    Task<AdminCategoryDto?> GetCategoryAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<int>> CreateCategoryAsync(CategoryInput input, CancellationToken cancellationToken = default);

    Task<Result> UpdateCategoryAsync(int id, CategoryInput input, CancellationToken cancellationToken = default);

    Task<Result> SaveServiceAsync(int? serviceId, ServiceInput input, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AdminCityDto>> GetLocationsAsync(CancellationToken cancellationToken = default);

    Task<Result> AddAreaAsync(int cityId, string name, CancellationToken cancellationToken = default);

    Task<Result> SetAreaActiveAsync(int areaId, bool isActive, CancellationToken cancellationToken = default);
}
