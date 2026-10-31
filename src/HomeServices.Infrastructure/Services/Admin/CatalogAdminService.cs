using System.Text.RegularExpressions;
using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Admin;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Entities;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Caching;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HomeServices.Infrastructure.Services.Admin;

internal sealed partial class CatalogAdminService(ApplicationDbContext db, IMemoryCache cache) : ICatalogAdminService
{
    // قيم الأيقونة واللون تُكتب داخل سمات HTML (class و style)، لذا تُقيَّد بصيغ صارمة
    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")]
    private static partial Regex SlugPattern();

    [GeneratedRegex("^fa-(solid|regular|brands) fa-[a-z0-9-]+$")]
    private static partial Regex IconPattern();

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$")]
    private static partial Regex ColorPattern();

    public async Task<IReadOnlyList<AdminCategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        await CategoryQuery(db.ServiceCategories).ToListAsync(cancellationToken);

    // التصفية على الكيان قبل الإسقاط إلى DTO، لأن EF لا يترجم شروطاً على خصائص الـ record الناتج
    public async Task<AdminCategoryDto?> GetCategoryAsync(int id, CancellationToken cancellationToken = default) =>
        await CategoryQuery(db.ServiceCategories.Where(c => c.Id == id)).FirstOrDefaultAsync(cancellationToken);

    public async Task<Result<int>> CreateCategoryAsync(CategoryInput input, CancellationToken cancellationToken = default)
    {
        var error = await ValidateCategoryAsync(input, excludeId: null, cancellationToken);
        if (error is not null)
        {
            return Result<int>.Failure(error);
        }

        var category = new ServiceCategory();
        Apply(category, input);
        db.ServiceCategories.Add(category);
        await db.SaveChangesAsync(cancellationToken);
        CatalogCache.Invalidate(cache);
        return Result<int>.Success(category.Id);
    }

    public async Task<Result> UpdateCategoryAsync(int id, CategoryInput input, CancellationToken cancellationToken = default)
    {
        var category = await db.ServiceCategories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null)
        {
            return Result.Failure("الفئة غير موجودة.");
        }

        var error = await ValidateCategoryAsync(input, excludeId: id, cancellationToken);
        if (error is not null)
        {
            return Result.Failure(error);
        }

        Apply(category, input);
        await db.SaveChangesAsync(cancellationToken);
        CatalogCache.Invalidate(cache);
        return Result.Success();
    }

    public async Task<Result> SaveServiceAsync(int? serviceId, ServiceInput input, CancellationToken cancellationToken = default)
    {
        var name = input.Name?.Trim() ?? string.Empty;
        var slug = input.Slug?.Trim().ToLowerInvariant() ?? string.Empty;

        if (name.Length is < 2 or > 100)
        {
            return Result.Failure("يجب أن يكون اسم الخدمة بين 2 و100 حرف.");
        }

        if (slug.Length > 100 || !SlugPattern().IsMatch(slug))
        {
            return Result.Failure("المعرّف (Slug) يجب أن يتكون من أحرف إنجليزية صغيرة وأرقام وشرطات فقط، مثل: leak-repair");
        }

        if (!await db.ServiceCategories.AnyAsync(c => c.Id == input.CategoryId, cancellationToken))
        {
            return Result.Failure("الفئة غير موجودة.");
        }

        if (await db.Services.AnyAsync(s => s.Slug == slug && s.Id != serviceId, cancellationToken))
        {
            return Result.Failure("هذا المعرّف مستخدم لخدمة أخرى.");
        }

        Service? service;
        if (serviceId is int id)
        {
            service = await db.Services.FirstOrDefaultAsync(s => s.Id == id && s.CategoryId == input.CategoryId, cancellationToken);
            if (service is null)
            {
                return Result.Failure("الخدمة غير موجودة.");
            }
        }
        else
        {
            service = new Service { CategoryId = input.CategoryId };
            db.Services.Add(service);
        }

        service.Name = name;
        service.Slug = slug;
        service.DisplayOrder = Math.Clamp(input.DisplayOrder, 0, 999);
        service.IsActive = input.IsActive;

        await db.SaveChangesAsync(cancellationToken);

        CatalogCache.Invalidate(cache);
        return Result.Success();
    }

    public async Task<IReadOnlyList<AdminCityDto>> GetLocationsAsync(CancellationToken cancellationToken = default) =>
        await db.Cities
            .AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new AdminCityDto(
                c.Id,
                c.Name,
                c.IsActive,
                c.Areas
                    .OrderBy(a => a.Id)
                    .Select(a => new AdminAreaDto(a.Id, a.Name, a.IsActive, db.ProviderServiceAreas.Count(pa => pa.AreaId == a.Id)))
                    .ToList()))
            .ToListAsync(cancellationToken);

    public async Task<Result> AddAreaAsync(int cityId, string name, CancellationToken cancellationToken = default)
    {
        var trimmed = name?.Trim() ?? string.Empty;
        if (trimmed.Length is < 2 or > 80)
        {
            return Result.Failure("يجب أن يكون اسم المنطقة بين 2 و80 حرف.");
        }

        if (!await db.Cities.AnyAsync(c => c.Id == cityId, cancellationToken))
        {
            return Result.Failure("المحافظة غير موجودة.");
        }

        if (await db.Areas.AnyAsync(a => a.CityId == cityId && a.Name == trimmed, cancellationToken))
        {
            return Result.Failure("هذه المنطقة موجودة مسبقاً في المحافظة.");
        }

        db.Areas.Add(new Area { CityId = cityId, Name = trimmed, IsActive = true });
        await db.SaveChangesAsync(cancellationToken);
        CatalogCache.Invalidate(cache);
        return Result.Success();
    }

    public async Task<Result> SetAreaActiveAsync(int areaId, bool isActive, CancellationToken cancellationToken = default)
    {
        var area = await db.Areas.FirstOrDefaultAsync(a => a.Id == areaId, cancellationToken);
        if (area is null)
        {
            return Result.Failure("المنطقة غير موجودة.");
        }

        area.IsActive = isActive;
        await db.SaveChangesAsync(cancellationToken);
        CatalogCache.Invalidate(cache);
        return Result.Success();
    }

    private static IQueryable<AdminCategoryDto> CategoryQuery(IQueryable<ServiceCategory> source) =>
        source
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new AdminCategoryDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.SearchKeywords,
                c.IconClass,
                c.AccentColor,
                c.DisplayOrder,
                c.IsActive,
                c.Services
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => new AdminServiceDto(
                        s.Id, s.CategoryId, s.Name, s.Slug, s.DisplayOrder, s.IsActive,
                        s.ProviderServices.Count(ps => ps.Provider.VerificationStatus == VerificationStatus.Approved)))
                    .ToList()));

    private async Task<string?> ValidateCategoryAsync(CategoryInput input, int? excludeId, CancellationToken cancellationToken)
    {
        var name = input.Name?.Trim() ?? string.Empty;
        var slug = input.Slug?.Trim().ToLowerInvariant() ?? string.Empty;

        if (name.Length is < 2 or > 80) return "يجب أن يكون اسم الفئة بين 2 و80 حرف.";
        if (slug.Length > 80 || !SlugPattern().IsMatch(slug)) return "المعرّف (Slug) يجب أن يتكون من أحرف إنجليزية صغيرة وأرقام وشرطات فقط، مثل: home-cleaning";
        if (!IconPattern().IsMatch(input.IconClass?.Trim() ?? string.Empty)) return "صيغة الأيقونة غير صحيحة. استخدم صيغة Font Awesome مثل: fa-solid fa-bolt";
        if (!ColorPattern().IsMatch(input.AccentColor?.Trim() ?? string.Empty)) return "اللون يجب أن يكون بصيغة Hex مثل: #0E5E6F";
        if (input.Description?.Length > 500) return "يجب ألا يتجاوز الوصف 500 حرف.";
        if (input.SearchKeywords?.Length > 500) return "يجب ألا تتجاوز الكلمات المفتاحية 500 حرف.";

        if (await db.ServiceCategories.AnyAsync(c => c.Slug == slug && c.Id != excludeId, cancellationToken))
        {
            return "هذا المعرّف مستخدم لفئة أخرى.";
        }

        return null;
    }

    private static void Apply(ServiceCategory category, CategoryInput input)
    {
        category.Name = input.Name.Trim();
        category.Slug = input.Slug.Trim().ToLowerInvariant();
        category.Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();
        category.SearchKeywords = string.IsNullOrWhiteSpace(input.SearchKeywords) ? null : input.SearchKeywords.Trim();
        category.IconClass = input.IconClass.Trim();
        category.AccentColor = input.AccentColor.Trim().ToUpperInvariant();
        category.DisplayOrder = Math.Clamp(input.DisplayOrder, 0, 999);
        category.IsActive = input.IsActive;
    }
}
