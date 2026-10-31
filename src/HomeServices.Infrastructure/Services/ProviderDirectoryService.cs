using HomeServices.Application.DTOs.Providers;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Infrastructure.Services;

internal sealed class ProviderDirectoryService(ApplicationDbContext db) : IProviderDirectoryService
{
    public async Task<IReadOnlyList<ProviderCardDto>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default)
    {
        return await db.ProviderProfiles
            .AsNoTracking()
            .WherePubliclyVisible()
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.ReviewsCount)
            .ThenByDescending(p => p.CompletedOrdersCount)
            .ThenByDescending(p => p.CreatedAt)
            .Take(count)
            .Select(p => new ProviderCardDto(
                p.Id,
                p.User.FullName,
                p.Headline,
                p.User.ProfileImagePath,
                p.City != null ? p.City.Name : null,
                p.Services.OrderBy(s => s.Price).Select(s => s.Service.Category.Name).FirstOrDefault(),
                p.YearsOfExperience,
                p.AverageRating,
                p.ReviewsCount,
                p.CompletedOrdersCount,
                p.IsAvailable,
                p.Services.OrderBy(s => s.Price).Select(s => (decimal?)s.Price).FirstOrDefault(),
                p.Services.OrderBy(s => s.Price).Select(s => (PriceUnit?)s.PriceUnit).FirstOrDefault()))
            .ToListAsync(cancellationToken);
    }
}
