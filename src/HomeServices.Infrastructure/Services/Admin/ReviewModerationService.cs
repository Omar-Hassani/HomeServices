using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Admin;
using HomeServices.Application.Interfaces;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServices.Infrastructure.Services.Admin;

internal sealed class ReviewModerationService(ApplicationDbContext db, ILogger<ReviewModerationService> logger) : IReviewModerationService
{
    private const int PageSize = 20;

    public async Task<PagedResult<AdminReviewRowDto>> GetReviewsAsync(ReviewVisibilityFilter visibility, byte? rating, int page, CancellationToken cancellationToken = default)
    {
        var reviews = db.Reviews.AsNoTracking();

        reviews = visibility switch
        {
            ReviewVisibilityFilter.Visible => reviews.Where(r => !r.IsHidden),
            ReviewVisibilityFilter.Hidden => reviews.Where(r => r.IsHidden),
            _ => reviews
        };

        if (rating is byte stars and >= 1 and <= 5)
        {
            reviews = reviews.Where(r => r.Rating == stars);
        }

        var total = await reviews.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var items = await reviews
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(r => new AdminReviewRowDto(
                r.Id,
                r.Rating,
                r.Comment,
                r.IsHidden,
                r.Customer.FullName,
                r.ProviderProfileId,
                r.Provider.User.FullName,
                r.ServiceRequestId,
                r.ServiceRequest.OrderNumber,
                r.ServiceRequest.Service.Name,
                r.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminReviewRowDto>(items, total, page, PageSize);
    }

    public async Task<Result> SetHiddenAsync(int reviewId, bool isHidden, CancellationToken cancellationToken = default)
    {
        var review = await db.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);
        if (review is null)
        {
            return Result.Failure("التقييم غير موجود.");
        }

        if (review.IsHidden == isHidden)
        {
            return Result.Success();
        }

        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            review.IsHidden = isHidden;
            await db.SaveChangesAsync(cancellationToken);

            await ProviderRatingCalculator.RecalculateAsync(db, review.ProviderProfileId, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });

        logger.LogInformation("Review {ReviewId} hidden={IsHidden}", reviewId, isHidden);
        return Result.Success();
    }
}
