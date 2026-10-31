using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Providers;

namespace HomeServices.Application.DTOs.Reviews;

public sealed record ReviewInput(byte Rating, string? Comment);

/// <summary>
/// صفحة تقييمات مقدم خدمة مع ملخصها (المتوسط وتوزيع النجوم).
/// </summary>
public sealed record ProviderReviewsPageDto
{
    public required int ProviderId { get; init; }
    public required string ProviderName { get; init; }
    public decimal AverageRating { get; init; }
    public int ReviewsCount { get; init; }

    /// <summary>
    /// عدد التقييمات لكل نجمة: الفهرس 0 لنجمة واحدة ... الفهرس 4 لخمس نجوم.
    /// </summary>
    public IReadOnlyList<int> RatingBreakdown { get; init; } = [0, 0, 0, 0, 0];

    public required PagedResult<ReviewDto> Reviews { get; init; }
}
