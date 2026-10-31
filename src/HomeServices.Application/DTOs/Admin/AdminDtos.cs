using HomeServices.Domain.Enums;

namespace HomeServices.Application.DTOs.Admin;

// ---------- لوحة الإحصائيات ----------

public sealed record AdminStatsDto
{
    public int UsersCount { get; init; }
    public int CustomersCount { get; init; }
    public int ProvidersCount { get; init; }
    public int ApprovedProvidersCount { get; init; }
    public int PendingProvidersCount { get; init; }
    public int NewUsersLast7Days { get; init; }
    public int OrdersCount { get; init; }
    public int OrdersLast7Days { get; init; }
    public int ReviewsCount { get; init; }
    public int HiddenReviewsCount { get; init; }
    public decimal PlatformAverageRating { get; init; }
    public IReadOnlyDictionary<OrderStatus, int> OrdersByStatus { get; init; } = new Dictionary<OrderStatus, int>();
}

// ---------- مقدمو الخدمات ----------

public sealed record AdminProviderRowDto(
    int Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? ProfileImagePath,
    string Headline,
    string? CityName,
    VerificationStatus Status,
    bool UserIsActive,
    int ServicesCount,
    int AreasCount,
    int CompletionPercent,
    decimal AverageRating,
    int CompletedOrdersCount,
    DateTime CreatedAt);

// ---------- المستخدمون ----------

public enum UserStatusFilter
{
    All = 0,
    Active = 1,
    Inactive = 2
}

public sealed record AdminUserRowDto(
    string Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? ProfileImagePath,
    IReadOnlyList<string> Roles,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt);

// ---------- الطلبات ----------

public sealed record AdminOrderRowDto(
    int Id,
    string OrderNumber,
    string ServiceName,
    string CategoryIcon,
    string CategoryAccentColor,
    string CustomerName,
    string ProviderName,
    string AreaName,
    string CityName,
    DateOnly ScheduledDate,
    OrderStatus Status,
    decimal? AgreedPrice,
    DateTime CreatedAt);

// ---------- التقييمات ----------

public enum ReviewVisibilityFilter
{
    All = 0,
    Visible = 1,
    Hidden = 2
}

public sealed record AdminReviewRowDto(
    int Id,
    byte Rating,
    string? Comment,
    bool IsHidden,
    string CustomerName,
    int ProviderId,
    string ProviderName,
    int OrderId,
    string OrderNumber,
    string ServiceName,
    DateTime CreatedAt);

// ---------- الفئات والخدمات والمناطق ----------

public sealed record AdminCategoryDto(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string? SearchKeywords,
    string IconClass,
    string AccentColor,
    int DisplayOrder,
    bool IsActive,
    IReadOnlyList<AdminServiceDto> Services);

public sealed record AdminServiceDto(int Id, int CategoryId, string Name, string Slug, int DisplayOrder, bool IsActive, int ProvidersCount);

public sealed record CategoryInput(
    string Name,
    string Slug,
    string? Description,
    string? SearchKeywords,
    string IconClass,
    string AccentColor,
    int DisplayOrder,
    bool IsActive);

public sealed record ServiceInput(int CategoryId, string Name, string Slug, int DisplayOrder, bool IsActive);

public sealed record AdminCityDto(int Id, string Name, bool IsActive, IReadOnlyList<AdminAreaDto> Areas);

public sealed record AdminAreaDto(int Id, string Name, bool IsActive, int ProvidersCount);
