using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Files;
using HomeServices.Application.DTOs.ProviderManagement;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Entities;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Infrastructure.Services;

internal sealed class ProviderProfileService(ApplicationDbContext db, IFileStorageService fileStorage) : IProviderProfileService
{
    private const int MinBioLength = 30;
    private const string ProfileNotFound = "لم يتم العثور على ملف مقدم الخدمة.";

    public async Task<ProviderDashboardDto?> GetDashboardAsync(string userId, CancellationToken cancellationToken = default)
    {
        var data = await db.ProviderProfiles
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                p.Id,
                p.User.FullName,
                p.User.ProfileImagePath,
                p.Headline,
                p.Bio,
                p.CityId,
                p.VerificationStatus,
                p.VerificationNote,
                p.IsAvailable,
                p.AverageRating,
                p.ReviewsCount,
                p.CompletedOrdersCount,
                ServicesCount = p.Services.Count(s => s.Service.IsActive),
                AreasCount = p.ServiceAreas.Count,
                PortfolioCount = p.PortfolioImages.Count,
                PendingOrdersCount = p.Requests.Count(r => r.Status == OrderStatus.Pending),
                ActiveOrdersCount = p.Requests.Count(r => r.Status == OrderStatus.Accepted || r.Status == OrderStatus.InProgress)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (data is null)
        {
            return null;
        }

        return new ProviderDashboardDto
        {
            ProfileId = data.Id,
            FullName = data.FullName,
            ProfileImagePath = data.ProfileImagePath,
            VerificationStatus = data.VerificationStatus,
            VerificationNote = data.VerificationNote,
            IsAvailable = data.IsAvailable,
            AverageRating = data.AverageRating,
            ReviewsCount = data.ReviewsCount,
            CompletedOrdersCount = data.CompletedOrdersCount,
            ServicesCount = data.ServicesCount,
            AreasCount = data.AreasCount,
            PortfolioCount = data.PortfolioCount,
            PendingOrdersCount = data.PendingOrdersCount,
            ActiveOrdersCount = data.ActiveOrdersCount,
            Checklist = BuildChecklist(data.Headline, data.Bio, data.CityId, data.ServicesCount, data.AreasCount,
                data.ProfileImagePath, data.PortfolioCount)
        };
    }

    public async Task<ProviderNavInfo?> GetNavInfoAsync(string userId, CancellationToken cancellationToken = default)
    {
        var dashboard = await GetDashboardAsync(userId, cancellationToken);
        return dashboard is null
            ? null
            : new ProviderNavInfo(dashboard.ProfileId, dashboard.VerificationStatus, dashboard.CompletionPercent, dashboard.PendingOrdersCount);
    }

    public async Task<ProviderProfileEditDto?> GetProfileForEditAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await db.ProviderProfiles
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new ProviderProfileEditDto(p.Headline, p.Bio, p.YearsOfExperience, p.CityId, p.IsAvailable))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Result> UpdateProfileAsync(string userId, ProviderProfileEditDto dto, CancellationToken cancellationToken = default)
    {
        var profile = await FindProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure(ProfileNotFound);
        }

        if (dto.CityId is int cityId && !await db.Cities.AnyAsync(c => c.Id == cityId && c.IsActive, cancellationToken))
        {
            return Result.Failure("المدينة المختارة غير صالحة.");
        }

        profile.Headline = dto.Headline.Trim();
        profile.Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio.Trim();
        profile.YearsOfExperience = dto.YearsOfExperience;
        profile.CityId = dto.CityId;
        profile.IsAvailable = dto.IsAvailable;

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> SetAvailabilityAsync(string userId, bool isAvailable, CancellationToken cancellationToken = default)
    {
        var profile = await FindProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure(ProfileNotFound);
        }

        profile.IsAvailable = isAvailable;
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ResubmitForReviewAsync(string userId, CancellationToken cancellationToken = default)
    {
        var dashboard = await GetDashboardAsync(userId, cancellationToken);
        if (dashboard is null)
        {
            return Result.Failure(ProfileNotFound);
        }

        if (dashboard.VerificationStatus != VerificationStatus.Rejected)
        {
            return Result.Failure("لا يمكن إعادة إرسال الملف للمراجعة في حالته الحالية.");
        }

        if (!dashboard.IsComplete)
        {
            return Result.Failure("أكمل البيانات المطلوبة في ملفك قبل إعادة إرساله للمراجعة.");
        }

        var profile = await FindProfileAsync(userId, cancellationToken);
        profile!.VerificationStatus = VerificationStatus.Pending;
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<IReadOnlyList<ManagedServiceDto>> GetServicesAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await db.ProviderServices
            .AsNoTracking()
            .Where(s => s.Provider.UserId == userId && s.Service.IsActive)
            .OrderBy(s => s.Service.Category.DisplayOrder)
            .ThenBy(s => s.Service.DisplayOrder)
            .Select(s => new ManagedServiceDto(
                s.Id,
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
    }

    public async Task<Result> SaveServiceAsync(string userId, ProviderServiceInput input, CancellationToken cancellationToken = default)
    {
        var profile = await FindProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure(ProfileNotFound);
        }

        if (!Enum.IsDefined(input.PriceUnit))
        {
            return Result.Failure("وحدة التسعير غير صالحة.");
        }

        if (!await db.Services.AnyAsync(s => s.Id == input.ServiceId && s.IsActive && s.Category.IsActive, cancellationToken))
        {
            return Result.Failure("الخدمة المختارة غير متاحة.");
        }

        var existing = await db.ProviderServices
            .FirstOrDefaultAsync(s => s.ProviderProfileId == profile.Id && s.ServiceId == input.ServiceId, cancellationToken);

        if (existing is null)
        {
            var count = await db.ProviderServices.CountAsync(s => s.ProviderProfileId == profile.Id, cancellationToken);
            if (count >= IProviderProfileService.MaxServices)
            {
                return Result.Failure($"لا يمكن إضافة أكثر من {IProviderProfileService.MaxServices} خدمة.");
            }

            existing = new ProviderService { ProviderProfileId = profile.Id, ServiceId = input.ServiceId };
            db.ProviderServices.Add(existing);
        }

        existing.Price = input.Price;
        existing.PriceUnit = input.PriceUnit;
        existing.Notes = string.IsNullOrWhiteSpace(input.Notes) ? null : input.Notes.Trim();

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RemoveServiceAsync(string userId, int providerServiceId, CancellationToken cancellationToken = default)
    {
        var service = await db.ProviderServices
            .FirstOrDefaultAsync(s => s.Id == providerServiceId && s.Provider.UserId == userId, cancellationToken);

        if (service is null)
        {
            return Result.Failure("الخدمة غير موجودة في ملفك.");
        }

        db.ProviderServices.Remove(service);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<IReadOnlySet<int>> GetAreaIdsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var ids = await db.ProviderServiceAreas
            .AsNoTracking()
            .Where(a => a.Provider.UserId == userId)
            .Select(a => a.AreaId)
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    public async Task<Result> UpdateAreasAsync(string userId, IReadOnlyCollection<int> areaIds, CancellationToken cancellationToken = default)
    {
        var profile = await FindProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure(ProfileNotFound);
        }

        var requested = areaIds.Distinct().ToList();
        if (requested.Count > IProviderProfileService.MaxAreas)
        {
            return Result.Failure($"يمكنك اختيار {IProviderProfileService.MaxAreas} منطقة كحد أقصى.");
        }

        var validCount = await db.Areas.CountAsync(a => requested.Contains(a.Id) && a.IsActive && a.City.IsActive, cancellationToken);
        if (validCount != requested.Count)
        {
            return Result.Failure("بعض المناطق المختارة غير صالحة.");
        }

        var current = await db.ProviderServiceAreas
            .Where(a => a.ProviderProfileId == profile.Id)
            .ToListAsync(cancellationToken);

        db.ProviderServiceAreas.RemoveRange(current.Where(a => !requested.Contains(a.AreaId)));

        var currentIds = current.Select(a => a.AreaId).ToHashSet();
        db.ProviderServiceAreas.AddRange(requested
            .Where(id => !currentIds.Contains(id))
            .Select(id => new ProviderServiceArea { ProviderProfileId = profile.Id, AreaId = id }));

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<IReadOnlyList<ManagedPortfolioImageDto>> GetPortfolioAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await db.ProviderPortfolioImages
            .AsNoTracking()
            .Where(i => i.Provider.UserId == userId)
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new ManagedPortfolioImageDto(i.Id, i.ImagePath, i.Caption))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result> AddPortfolioImageAsync(string userId, FileUpload image, string? caption, CancellationToken cancellationToken = default)
    {
        var profile = await FindProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure(ProfileNotFound);
        }

        var images = await db.ProviderPortfolioImages
            .Where(i => i.ProviderProfileId == profile.Id)
            .Select(i => i.DisplayOrder)
            .ToListAsync(cancellationToken);

        if (images.Count >= IProviderProfileService.MaxPortfolioImages)
        {
            return Result.Failure($"وصلت للحد الأقصى ({IProviderProfileService.MaxPortfolioImages} صورة). احذف صورة لإضافة أخرى.");
        }

        var saved = await fileStorage.SaveImageAsync(image, StorageFolders.Portfolio, cancellationToken);
        if (!saved.Succeeded)
        {
            return Result.Failure(saved.Errors);
        }

        db.ProviderPortfolioImages.Add(new ProviderPortfolioImage
        {
            ProviderProfileId = profile.Id,
            ImagePath = saved.Value!,
            Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim(),
            DisplayOrder = images.Count == 0 ? 1 : images.Max() + 1,
            CreatedAt = DateTime.UtcNow
        });

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // لا نترك ملفاً يتيماً على القرص إذا فشل الحفظ في قاعدة البيانات
            await fileStorage.DeleteAsync(saved.Value, cancellationToken);
            throw;
        }

        return Result.Success();
    }

    public async Task<Result> RemovePortfolioImageAsync(string userId, int imageId, CancellationToken cancellationToken = default)
    {
        var image = await db.ProviderPortfolioImages
            .FirstOrDefaultAsync(i => i.Id == imageId && i.Provider.UserId == userId, cancellationToken);

        if (image is null)
        {
            return Result.Failure("الصورة غير موجودة في معرض أعمالك.");
        }

        db.ProviderPortfolioImages.Remove(image);
        await db.SaveChangesAsync(cancellationToken);
        await fileStorage.DeleteAsync(image.ImagePath, cancellationToken);
        return Result.Success();
    }

    private Task<ProviderProfile?> FindProfileAsync(string userId, CancellationToken cancellationToken) =>
        db.ProviderProfiles.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

    private static List<ProfileChecklistItem> BuildChecklist(
        string headline, string? bio, int? cityId, int servicesCount, int areasCount, string? profileImagePath, int portfolioCount) =>
    [
        new("headline", "العنوان المهني", !string.IsNullOrWhiteSpace(headline), IsRequired: true),
        new("bio", $"نبذة تعريفية ({MinBioLength} حرفاً على الأقل)", (bio?.Trim().Length ?? 0) >= MinBioLength, IsRequired: true),
        new("city", "المدينة الرئيسية", cityId.HasValue, IsRequired: true),
        new("services", "خدمة واحدة على الأقل مع السعر", servicesCount > 0, IsRequired: true),
        new("areas", "منطقة عمل واحدة على الأقل", areasCount > 0, IsRequired: true),
        new("photo", "صورة شخصية واضحة", !string.IsNullOrEmpty(profileImagePath), IsRequired: false),
        new("portfolio", "صور من أعمالك السابقة", portfolioCount > 0, IsRequired: false)
    ];
}
