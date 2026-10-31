using System.Linq.Expressions;
using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Providers;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Entities;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Infrastructure.Services;

internal sealed class ProviderDirectoryService(ApplicationDbContext db) : IProviderDirectoryService
{
    private const int MaxPageSize = 48;
    private const int MaxSearchTerms = 5;
    private const int RecentReviewsCount = 6;

    public async Task<IReadOnlyList<ProviderCardDto>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default)
    {
        Expression<Func<ProviderService, bool>> anyActiveService = ps => ps.Service.IsActive;

        return await ToCards(
                db.ProviderProfiles
                    .AsNoTracking()
                    .WherePubliclyVisible()
                    .OrderByDescending(p => p.AverageRating)
                    .ThenByDescending(p => p.ReviewsCount)
                    .ThenByDescending(p => p.CompletedOrdersCount)
                    .ThenByDescending(p => p.CreatedAt)
                    .Take(count),
                anyActiveService)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<ProviderCardDto>> SearchAsync(ProviderSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        var pageSize = Math.Clamp(criteria.PageSize, 1, MaxPageSize);
        var serviceFilter = BuildServiceFilter(criteria);

        var query = db.ProviderProfiles.AsNoTracking().WherePubliclyVisible();

        if (criteria.CategoryId.HasValue || criteria.ServiceId.HasValue || criteria.MinPrice.HasValue || criteria.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Services.AsQueryable().Any(serviceFilter));
        }

        if (criteria.AreaId is int areaId)
        {
            query = query.Where(p => p.ServiceAreas.Any(a => a.AreaId == areaId));
        }
        else if (criteria.CityId is int cityId)
        {
            query = query.Where(p => p.CityId == cityId || p.ServiceAreas.Any(a => a.Area.CityId == cityId));
        }

        if (criteria.MinRating is decimal minRating)
        {
            query = query.Where(p => p.AverageRating >= minRating);
        }

        if (criteria.AvailableOnly)
        {
            query = query.Where(p => p.IsAvailable);
        }

        // كل كلمة في البحث يجب أن تطابق أحد الحقول (الاسم، العنوان المهني، الخدمات، الفئات وكلماتها المفتاحية، المدينة)
        foreach (var term in ExtractSearchTerms(criteria.Query))
        {
            query = query.Where(p =>
                p.User.FullName.Contains(term) ||
                p.Headline.Contains(term) ||
                (p.City != null && p.City.Name.Contains(term)) ||
                p.Services.Any(s =>
                    s.Service.Name.Contains(term) ||
                    s.Service.Category.Name.Contains(term) ||
                    (s.Service.Category.SearchKeywords != null && s.Service.Category.SearchKeywords.Contains(term))));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var page = Math.Clamp(criteria.Page, 1, totalPages);

        var items = await ToCards(ApplySort(query, criteria.Sort, serviceFilter).Skip((page - 1) * pageSize).Take(pageSize), serviceFilter)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProviderCardDto>(items, totalCount, page, pageSize);
    }

    public async Task<ProviderDetailsDto?> GetDetailsAsync(int providerId, string? viewerUserId, bool viewerIsAdmin, CancellationToken cancellationToken = default)
    {
        var profile = await db.ProviderProfiles
            .AsNoTracking()
            .Where(p => p.Id == providerId)
            .Select(p => new
            {
                p.Id,
                p.UserId,
                p.User.FullName,
                p.User.ProfileImagePath,
                p.User.IsActive,
                MemberSince = p.User.CreatedAt,
                p.Headline,
                p.Bio,
                CityName = p.City != null ? p.City.Name : null,
                p.YearsOfExperience,
                p.AverageRating,
                p.ReviewsCount,
                p.CompletedOrdersCount,
                p.IsAvailable,
                p.VerificationStatus
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (profile is null)
        {
            return null;
        }

        var isPublic = profile.VerificationStatus == VerificationStatus.Approved && profile.IsActive;
        var isOwner = viewerUserId is not null && viewerUserId == profile.UserId;
        if (!isPublic && !isOwner && !viewerIsAdmin)
        {
            return null;
        }

        var services = await db.ProviderServices
            .AsNoTracking()
            .Where(s => s.ProviderProfileId == providerId && s.Service.IsActive)
            .OrderBy(s => s.Service.Category.DisplayOrder)
            .ThenBy(s => s.Service.DisplayOrder)
            .Select(s => new ProviderServiceItemDto(
                s.ServiceId,
                s.Service.Name,
                s.Service.CategoryId,
                s.Service.Category.Name,
                s.Service.Category.IconClass,
                s.Service.Category.AccentColor,
                s.Price,
                s.PriceUnit,
                s.Notes))
            .ToListAsync(cancellationToken);

        var areas = await db.ProviderServiceAreas
            .AsNoTracking()
            .Where(a => a.ProviderProfileId == providerId && a.Area.IsActive)
            .OrderBy(a => a.Area.City.DisplayOrder)
            .ThenBy(a => a.AreaId)
            .Select(a => new { CityName = a.Area.City.Name, AreaName = a.Area.Name })
            .ToListAsync(cancellationToken);

        var portfolio = await db.ProviderPortfolioImages
            .AsNoTracking()
            .Where(i => i.ProviderProfileId == providerId)
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new PortfolioImageDto(i.ImagePath, i.Caption))
            .ToListAsync(cancellationToken);

        var visibleReviews = db.Reviews.AsNoTracking().Where(r => r.ProviderProfileId == providerId && !r.IsHidden);

        var breakdown = await visibleReviews
            .GroupBy(r => r.Rating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var recentReviews = await visibleReviews
            .OrderByDescending(r => r.CreatedAt)
            .Take(RecentReviewsCount)
            .Select(r => new { r.Customer.FullName, r.Rating, r.Comment, ServiceName = r.ServiceRequest.Service.Name, r.CreatedAt })
            .ToListAsync(cancellationToken);

        return new ProviderDetailsDto
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Headline = profile.Headline,
            Bio = profile.Bio,
            ProfileImagePath = profile.ProfileImagePath,
            CityName = profile.CityName,
            YearsOfExperience = profile.YearsOfExperience,
            AverageRating = profile.AverageRating,
            ReviewsCount = profile.ReviewsCount,
            CompletedOrdersCount = profile.CompletedOrdersCount,
            IsAvailable = profile.IsAvailable,
            MemberSince = profile.MemberSince,
            VerificationStatus = profile.VerificationStatus,
            IsPubliclyVisible = isPublic,
            Services = services,
            Areas = areas
                .GroupBy(a => a.CityName)
                .Select(g => new ProviderAreaGroupDto(g.Key, g.Select(a => a.AreaName).ToList()))
                .ToList(),
            Portfolio = portfolio,
            RatingBreakdown = Enumerable.Range(1, 5)
                .Select(star => breakdown.FirstOrDefault(b => b.Rating == star)?.Count ?? 0)
                .ToList(),
            RecentReviews = recentReviews
                .Select(r => new ReviewDto(NameFormatter.ToPrivateDisplayName(r.FullName), r.Rating, r.Comment, r.ServiceName, r.CreatedAt))
                .ToList()
        };
    }

    /// <summary>
    /// شرط مطابقة خدمات مقدم الخدمة لفلاتر الفئة والخدمة والسعر. يُستخدم للتصفية ولحساب "السعر يبدأ من".
    /// </summary>
    private static Expression<Func<ProviderService, bool>> BuildServiceFilter(ProviderSearchCriteria criteria)
    {
        Expression<Func<ProviderService, bool>> filter = ps => ps.Service.IsActive;

        if (criteria.ServiceId is int serviceId)
        {
            filter = filter.And(ps => ps.ServiceId == serviceId);
        }
        else if (criteria.CategoryId is int categoryId)
        {
            filter = filter.And(ps => ps.Service.CategoryId == categoryId);
        }

        if (criteria.MinPrice is decimal minPrice)
        {
            filter = filter.And(ps => ps.Price >= minPrice);
        }

        if (criteria.MaxPrice is decimal maxPrice)
        {
            filter = filter.And(ps => ps.Price <= maxPrice);
        }

        return filter;
    }

    private static IQueryable<ProviderProfile> ApplySort(
        IQueryable<ProviderProfile> query,
        ProviderSortOrder sort,
        Expression<Func<ProviderService, bool>> serviceFilter) => sort switch
    {
        ProviderSortOrder.HighestRated => query
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.ReviewsCount)
            .ThenBy(p => p.Id),
        ProviderSortOrder.LowestPrice => query
            .OrderBy(p => p.Services.AsQueryable().Where(serviceFilter).Min(s => (decimal?)s.Price) == null)
            .ThenBy(p => p.Services.AsQueryable().Where(serviceFilter).Min(s => (decimal?)s.Price))
            .ThenBy(p => p.Id),
        ProviderSortOrder.HighestPrice => query
            .OrderByDescending(p => p.Services.AsQueryable().Where(serviceFilter).Min(s => (decimal?)s.Price))
            .ThenBy(p => p.Id),
        ProviderSortOrder.MostExperienced => query
            .OrderByDescending(p => p.YearsOfExperience)
            .ThenBy(p => p.Id),
        ProviderSortOrder.Newest => query
            .OrderByDescending(p => p.CreatedAt)
            .ThenBy(p => p.Id),
        _ => query
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.ReviewsCount)
            .ThenByDescending(p => p.CompletedOrdersCount)
            .ThenBy(p => p.Id)
    };

    /// <summary>
    /// "السعر يبدأ من" والفئة الرئيسية يُحسبان من أرخص خدمة مطابقة للفلاتر الحالية.
    /// </summary>
    private static IQueryable<ProviderCardDto> ToCards(IQueryable<ProviderProfile> query, Expression<Func<ProviderService, bool>> serviceFilter) =>
        query.Select(p => new ProviderCardDto(
            p.Id,
            p.User.FullName,
            p.Headline,
            p.User.ProfileImagePath,
            p.City != null ? p.City.Name : null,
            p.Services.AsQueryable().Where(serviceFilter).OrderBy(s => s.Price).Select(s => s.Service.Category.Name).FirstOrDefault(),
            p.YearsOfExperience,
            p.AverageRating,
            p.ReviewsCount,
            p.CompletedOrdersCount,
            p.IsAvailable,
            p.Services.AsQueryable().Where(serviceFilter).OrderBy(s => s.Price).Select(s => (decimal?)s.Price).FirstOrDefault(),
            p.Services.AsQueryable().Where(serviceFilter).OrderBy(s => s.Price).Select(s => (PriceUnit?)s.PriceUnit).FirstOrDefault()));

    /// <summary>
    /// تقسيم نص البحث إلى كلمات، مع حذف "ال" التعريف من بداية الكلمات الطويلة لتوسيع المطابقة.
    /// </summary>
    private static IEnumerable<string> ExtractSearchTerms(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        return query
            .Split([' ', '\t', ',', '،', '-'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(t => t.Length > 4 && t.StartsWith("ال") ? t[2..] : t)
            .Where(t => t.Length >= 2)
            .Distinct()
            .Take(MaxSearchTerms);
    }
}
