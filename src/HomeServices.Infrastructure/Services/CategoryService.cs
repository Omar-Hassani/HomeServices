using HomeServices.Application.DTOs.Catalog;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Infrastructure.Services;

internal sealed class CategoryService(ApplicationDbContext db) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryDto>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
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
}
