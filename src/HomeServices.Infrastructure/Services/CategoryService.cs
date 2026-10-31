using HomeServices.Application.DTOs.Catalog;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Entities;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Caching;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HomeServices.Infrastructure.Services;

internal sealed class CategoryService(ApplicationDbContext db, IMemoryCache cache) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryDto>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default) =>
        (await cache.GetOrCreateAsync(CatalogCache.ActiveCategories, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CatalogCache.Duration;
            return LoadActiveCategoriesAsync(cancellationToken);
        }))!;

    public async Task<IReadOnlyList<CategoryDetailsDto>> GetCatalogAsync(CancellationToken cancellationToken = default) =>
        (await cache.GetOrCreateAsync(CatalogCache.Catalog, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CatalogCache.Duration;
            IReadOnlyList<CategoryDetailsDto> catalog = await ActiveCategoryDetails(db.ServiceCategories.AsNoTracking()).ToListAsync(cancellationToken);
            return catalog;
        }))!;

    private async Task<IReadOnlyList<CategoryDto>> LoadActiveCategoriesAsync(CancellationToken cancellationToken)
    {
        return await db.ServiceCategories
            .AsNoTracking()
            .Where(c => c.IsActive && c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.IconClass,
                c.AccentColor,
                c.Services
                    .Where(s => s.IsActive)
                    .SelectMany(s => s.ProviderServices)
                    .Where(ps => ps.Provider.VerificationStatus == VerificationStatus.Approved && ps.Provider.User.IsActive)
                    .Select(ps => ps.ProviderProfileId)
                    .Distinct()
                    .Count()))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryDetailsDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var normalized = slug.Trim().ToLowerInvariant();
        return await ActiveCategoryDetails(db.ServiceCategories.AsNoTracking().Where(c => c.Slug == normalized))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static IQueryable<CategoryDetailsDto> ActiveCategoryDetails(IQueryable<ServiceCategory> query) =>
        query
            .Where(c => c.IsActive && c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDetailsDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.IconClass,
                c.AccentColor,
                c.Services
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => new ServiceDto(
                        s.Id,
                        s.CategoryId,
                        s.Name,
                        s.Slug,
                        s.ProviderServices.Count(ps =>
                            ps.Provider.VerificationStatus == VerificationStatus.Approved && ps.Provider.User.IsActive)))
                    .ToList()));
}
