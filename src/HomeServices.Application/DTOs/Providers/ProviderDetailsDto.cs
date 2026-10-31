using HomeServices.Domain.Enums;

namespace HomeServices.Application.DTOs.Providers;

public sealed record ProviderDetailsDto
{
    public required int Id { get; init; }
    public required string FullName { get; init; }
    public required string Headline { get; init; }
    public string? Bio { get; init; }
    public string? ProfileImagePath { get; init; }
    public string? CityName { get; init; }
    public int YearsOfExperience { get; init; }
    public decimal AverageRating { get; init; }
    public int ReviewsCount { get; init; }
    public int CompletedOrdersCount { get; init; }
    public bool IsAvailable { get; init; }
    public DateTime MemberSince { get; init; }
    public VerificationStatus VerificationStatus { get; init; }

    /// <summary>
    /// هل الملف ظاهر للعموم؟ يكون false عندما يعاين صاحب الملف أو المدير ملفاً غير موثّق.
    /// </summary>
    public bool IsPubliclyVisible { get; init; }

    public IReadOnlyList<ProviderServiceItemDto> Services { get; init; } = [];
    public IReadOnlyList<ProviderAreaGroupDto> Areas { get; init; } = [];
    public IReadOnlyList<PortfolioImageDto> Portfolio { get; init; } = [];

    /// <summary>
    /// عدد التقييمات لكل نجمة: الفهرس 0 لنجمة واحدة ... الفهرس 4 لخمس نجوم.
    /// </summary>
    public IReadOnlyList<int> RatingBreakdown { get; init; } = [0, 0, 0, 0, 0];

    public IReadOnlyList<ReviewDto> RecentReviews { get; init; } = [];

    public decimal? StartingPrice => Services.Count == 0 ? null : Services.Min(s => s.Price);
}

public sealed record ProviderServiceItemDto(
    int ServiceId,
    string ServiceName,
    int CategoryId,
    string CategoryName,
    string CategoryIcon,
    string CategoryAccentColor,
    decimal Price,
    PriceUnit PriceUnit,
    string? Notes);

public sealed record ProviderAreaGroupDto(string CityName, IReadOnlyList<string> Areas);

public sealed record PortfolioImageDto(string ImagePath, string? Caption);

public sealed record ReviewDto(string CustomerDisplayName, byte Rating, string? Comment, string ServiceName, DateTime CreatedAt);
