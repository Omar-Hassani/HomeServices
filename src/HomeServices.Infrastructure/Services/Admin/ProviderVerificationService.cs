using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Admin;
using HomeServices.Application.DTOs.Notifications;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Entities;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServices.Infrastructure.Services.Admin;

internal sealed class ProviderVerificationService(
    ApplicationDbContext db,
    INotificationService notifications,
    ILogger<ProviderVerificationService> logger) : IProviderVerificationService
{
    private const int PageSize = 15;
    private const int MinNoteLength = 5;
    private const int MaxNoteLength = 500;

    public async Task<PagedResult<AdminProviderRowDto>> GetProvidersAsync(VerificationStatus? status, string? query, int page, CancellationToken cancellationToken = default)
    {
        var providers = db.ProviderProfiles.AsNoTracking();

        if (status is VerificationStatus s)
        {
            providers = providers.Where(p => p.VerificationStatus == s);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            providers = providers.Where(p =>
                p.User.FullName.Contains(term) || p.User.Email!.Contains(term) ||
                (p.User.PhoneNumber != null && p.User.PhoneNumber.Contains(term)) || p.Headline.Contains(term));
        }

        var total = await providers.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var rows = await providers
            .OrderBy(p => p.VerificationStatus == VerificationStatus.Pending ? 0 : 1)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new
            {
                p.Id,
                p.User.FullName,
                Email = p.User.Email!,
                p.User.PhoneNumber,
                p.User.ProfileImagePath,
                p.Headline,
                p.Bio,
                p.CityId,
                CityName = p.City != null ? p.City.Name : null,
                p.VerificationStatus,
                p.User.IsActive,
                ServicesCount = p.Services.Count(x => x.Service.IsActive),
                AreasCount = p.ServiceAreas.Count,
                PortfolioCount = p.PortfolioImages.Count,
                p.AverageRating,
                p.CompletedOrdersCount,
                p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(r => new AdminProviderRowDto(
                r.Id, r.FullName, r.Email, r.PhoneNumber, r.ProfileImagePath, r.Headline, r.CityName, r.VerificationStatus, r.IsActive,
                r.ServicesCount, r.AreasCount,
                ProfileChecklist.CompletionPercent(ProfileChecklist.Build(r.Headline, r.Bio, r.CityId, r.ServicesCount, r.AreasCount, r.ProfileImagePath, r.PortfolioCount)),
                r.AverageRating, r.CompletedOrdersCount, r.CreatedAt))
            .ToList();

        return new PagedResult<AdminProviderRowDto>(items, total, page, PageSize);
    }

    public Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default) =>
        db.ProviderProfiles.CountAsync(p => p.VerificationStatus == VerificationStatus.Pending, cancellationToken);

    public async Task<Result> ApproveAsync(int providerId, string? note, CancellationToken cancellationToken = default)
    {
        var provider = await LoadAsync(providerId, cancellationToken);
        if (provider is null)
        {
            return Result.Failure("مقدم الخدمة غير موجود.");
        }

        if (provider.VerificationStatus == VerificationStatus.Approved)
        {
            return Result.Failure("الملف موثّق مسبقاً.");
        }

        var checklist = ProfileChecklist.Build(provider.Headline, provider.Bio, provider.CityId,
            provider.Services.Count, provider.ServiceAreas.Count, provider.User.ProfileImagePath, provider.PortfolioImages.Count);

        if (!ProfileChecklist.IsComplete(checklist))
        {
            return Result.Failure("لا يمكن توثيق ملف غير مكتمل. يجب أن يحتوي على العنوان المهني والنبذة والمدينة وخدمة ومنطقة عمل واحدة على الأقل.");
        }

        if (note?.Length > MaxNoteLength)
        {
            return Result.Failure($"يجب ألا تتجاوز الملاحظة {MaxNoteLength} حرف.");
        }

        var wasSuspended = provider.VerificationStatus == VerificationStatus.Suspended;
        provider.VerificationStatus = VerificationStatus.Approved;
        provider.VerificationNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        await db.SaveChangesAsync(cancellationToken);

        await NotifyAsync(provider,
            wasSuspended ? "تمت إعادة تفعيل ملفك" : "تهانينا! تم توثيق ملفك",
            "ملفك ظاهر الآن للعملاء في نتائج البحث ويمكنهم إرسال الطلبات إليك.",
            cancellationToken);

        logger.LogInformation("Provider {ProviderId} approved", providerId);
        return Result.Success();
    }

    public Task<Result> RejectAsync(int providerId, string note, CancellationToken cancellationToken = default) =>
        ChangeWithNoteAsync(providerId, note, VerificationStatus.Rejected, [VerificationStatus.Pending],
            "لم تتم الموافقة على ملفك", "يمكن رفض الملفات قيد المراجعة فقط.", cancellationToken);

    public Task<Result> SuspendAsync(int providerId, string note, CancellationToken cancellationToken = default) =>
        ChangeWithNoteAsync(providerId, note, VerificationStatus.Suspended, [VerificationStatus.Approved],
            "تم إيقاف ملفك مؤقتاً", "يمكن إيقاف الملفات الموثّقة فقط.", cancellationToken);

    private async Task<Result> ChangeWithNoteAsync(
        int providerId, string note, VerificationStatus target, VerificationStatus[] allowedFrom,
        string notificationTitle, string invalidStateMessage, CancellationToken cancellationToken)
    {
        var trimmed = note?.Trim() ?? string.Empty;
        if (trimmed.Length < MinNoteLength || trimmed.Length > MaxNoteLength)
        {
            return Result.Failure($"يرجى كتابة ملاحظة توضح السبب لمقدم الخدمة (بين {MinNoteLength} و{MaxNoteLength} حرف).");
        }

        var provider = await LoadAsync(providerId, cancellationToken);
        if (provider is null)
        {
            return Result.Failure("مقدم الخدمة غير موجود.");
        }

        if (!allowedFrom.Contains(provider.VerificationStatus))
        {
            return Result.Failure(invalidStateMessage);
        }

        provider.VerificationStatus = target;
        provider.VerificationNote = trimmed;
        await db.SaveChangesAsync(cancellationToken);

        await NotifyAsync(provider, notificationTitle, $"ملاحظة الإدارة: {trimmed}", cancellationToken);
        logger.LogInformation("Provider {ProviderId} set to {Status}", providerId, target);
        return Result.Success();
    }

    private Task<ProviderProfile?> LoadAsync(int providerId, CancellationToken cancellationToken) =>
        db.ProviderProfiles
            .Include(p => p.User)
            .Include(p => p.Services.Where(s => s.Service.IsActive))
            .Include(p => p.ServiceAreas)
            .Include(p => p.PortfolioImages)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == providerId, cancellationToken);

    private Task NotifyAsync(ProviderProfile provider, string title, string message, CancellationToken cancellationToken) =>
        notifications.NotifyAsync(new NotificationMessage(
            provider.UserId, NotificationType.ProviderVerification, title, message, NotificationLinks.ProviderDashboard), cancellationToken);
}
