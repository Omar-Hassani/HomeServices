using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Providers;
using HomeServices.Application.DTOs.Reviews;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Entities;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServices.Infrastructure.Services;

internal sealed class ReviewService(ApplicationDbContext db, ILogger<ReviewService> logger) : IReviewService
{
    private const int PageSize = 10;

    public async Task<Result> CreateAsync(string customerId, int orderId, ReviewInput input, CancellationToken cancellationToken = default)
    {
        if (input.Rating is < 1 or > 5)
        {
            return Result.Failure("يرجى اختيار تقييم من نجمة إلى خمس نجوم.");
        }

        var comment = string.IsNullOrWhiteSpace(input.Comment) ? null : input.Comment.Trim();
        if (comment?.Length > IReviewService.MaxCommentLength)
        {
            return Result.Failure($"يجب ألا يتجاوز التعليق {IReviewService.MaxCommentLength} حرف.");
        }

        var order = await db.ServiceRequests
            .AsNoTracking()
            .Where(r => r.Id == orderId && r.CustomerId == customerId)
            .Select(r => new { r.Id, r.Status, r.ProviderProfileId, HasReview = r.Review != null })
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            return Result.Failure("الطلب غير موجود.");
        }

        if (order.Status != OrderStatus.Completed)
        {
            return Result.Failure("يمكن تقييم الخدمة بعد إتمام الطلب فقط.");
        }

        if (order.HasReview)
        {
            return Result.Failure("لقد قيّمت هذا الطلب مسبقاً.");
        }

        var strategy = db.Database.CreateExecutionStrategy();
        try
        {
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

                db.Reviews.Add(new Review
                {
                    ServiceRequestId = order.Id,
                    CustomerId = customerId,
                    ProviderProfileId = order.ProviderProfileId,
                    Rating = input.Rating,
                    Comment = comment
                });
                await db.SaveChangesAsync(cancellationToken);

                await ProviderRatingCalculator.RecalculateAsync(db, order.ProviderProfileId, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            });
        }
        catch (DbUpdateException ex)
        {
            // الفهرس الفريد على ServiceRequestId يمنع تقييمين متزامنين لنفس الطلب
            db.ChangeTracker.Clear();
            if (!await db.Reviews.AnyAsync(r => r.ServiceRequestId == order.Id, CancellationToken.None))
            {
                throw;
            }

            logger.LogWarning(ex, "Duplicate review attempt for order {OrderId}", order.Id);
            return Result.Failure("لقد قيّمت هذا الطلب مسبقاً.");
        }

        logger.LogInformation("Review added for order {OrderId} with rating {Rating}", order.Id, input.Rating);
        return Result.Success();
    }

    public async Task<ProviderReviewsPageDto?> GetPublicReviewsAsync(int providerId, int page, CancellationToken cancellationToken = default)
    {
        var isVisible = await db.ProviderProfiles.AsNoTracking().WherePubliclyVisible().AnyAsync(p => p.Id == providerId, cancellationToken);
        return isVisible ? await BuildPageAsync(providerId, page, cancellationToken) : null;
    }

    public async Task<ProviderReviewsPageDto?> GetOwnReviewsAsync(string providerUserId, int page, CancellationToken cancellationToken = default)
    {
        var providerId = await db.ProviderProfiles
            .AsNoTracking()
            .Where(p => p.UserId == providerUserId)
            .Select(p => (int?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return providerId is null ? null : await BuildPageAsync(providerId.Value, page, cancellationToken);
    }

    private async Task<ProviderReviewsPageDto> BuildPageAsync(int providerId, int page, CancellationToken cancellationToken)
    {
        var provider = await db.ProviderProfiles
            .AsNoTracking()
            .Where(p => p.Id == providerId)
            .Select(p => new { p.User.FullName, p.AverageRating, p.ReviewsCount })
            .FirstAsync(cancellationToken);

        var visible = db.Reviews.AsNoTracking().Where(r => r.ProviderProfileId == providerId && !r.IsHidden);

        var breakdown = await visible
            .GroupBy(r => r.Rating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var total = breakdown.Sum(b => b.Count);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var rows = await visible
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(r => new { r.Customer.FullName, r.Rating, r.Comment, ServiceName = r.ServiceRequest.Service.Name, r.CreatedAt })
            .ToListAsync(cancellationToken);

        var items = rows
            .Select(r => new ReviewDto(NameFormatter.ToPrivateDisplayName(r.FullName), r.Rating, r.Comment, r.ServiceName, r.CreatedAt))
            .ToList();

        return new ProviderReviewsPageDto
        {
            ProviderId = providerId,
            ProviderName = provider.FullName,
            AverageRating = provider.AverageRating,
            ReviewsCount = provider.ReviewsCount,
            RatingBreakdown = Enumerable.Range(1, 5).Select(star => breakdown.FirstOrDefault(b => b.Rating == star)?.Count ?? 0).ToList(),
            Reviews = new PagedResult<ReviewDto>(items, total, page, PageSize)
        };
    }
}
