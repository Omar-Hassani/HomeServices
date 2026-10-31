using HomeServices.Application.DTOs.Reviews;

namespace HomeServices.Web.ViewModels.Shared;

public class ReviewsPageViewModel
{
    public required ProviderReviewsPageDto Reviews { get; init; }
    public required PaginationViewModel Pagination { get; init; }

    public RatingSummaryViewModel Summary => new()
    {
        AverageRating = Reviews.AverageRating,
        ReviewsCount = Reviews.ReviewsCount,
        Breakdown = Reviews.RatingBreakdown
    };
}
