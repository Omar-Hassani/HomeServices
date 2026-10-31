using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Notifications;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Entities;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServices.Infrastructure.Services;

internal sealed class NotificationService(
    ApplicationDbContext db,
    IEnumerable<INotificationChannel> channels,
    ILogger<NotificationService> logger) : INotificationService
{
    private const int PageSize = 15;
    private const int MaxTitleLength = 150;
    private const int MaxMessageLength = 500;

    public async Task NotifyAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        var entity = new Notification
        {
            UserId = message.UserId,
            Type = message.Type,
            Title = Truncate(message.Title, MaxTitleLength),
            Message = Truncate(message.Message, MaxMessageLength),
            Link = message.Link,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            db.Notifications.Add(entity);
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to store {Type} notification for user {UserId}", message.Type, message.UserId);
            return;
        }
        finally
        {
            // لا نترك الإشعار متتبَّعاً في DbContext المشترك، حتى لا يُعاد حفظه مع أي عملية لاحقة في نفس الطلب
            db.Entry(entity).State = EntityState.Detached;
        }

        foreach (var channel in channels)
        {
            try
            {
                await channel.SendAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Notification channel {Channel} failed for user {UserId}", channel.GetType().Name, message.UserId);
            }
        }
    }

    public Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default) =>
        db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

    public async Task<IReadOnlyList<NotificationDto>> GetRecentAsync(string userId, int count, CancellationToken cancellationToken = default) =>
        await ForUser(userId).Take(count).ToListAsync(cancellationToken);

    public async Task<PagedResult<NotificationDto>> GetPageAsync(string userId, int page, CancellationToken cancellationToken = default)
    {
        var total = await db.Notifications.CountAsync(n => n.UserId == userId, cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var items = await ForUser(userId).Skip((page - 1) * PageSize).Take(PageSize).ToListAsync(cancellationToken);
        return new PagedResult<NotificationDto>(items, total, page, PageSize);
    }

    public async Task<string?> OpenAsync(string userId, int notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await db.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, cancellationToken);
        if (notification is null)
        {
            return null;
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
        }

        return notification.Link;
    }

    public Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default) =>
        db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt, DateTime.UtcNow), cancellationToken);

    private IQueryable<NotificationDto> ForUser(string userId) =>
        db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ThenByDescending(n => n.Id)
            .Select(n => new NotificationDto(n.Id, n.Type, n.Title, n.Message, n.IsRead, n.CreatedAt));

    private static string Truncate(string value, int max) => value.Length <= max ? value : value[..(max - 1)] + "…";
}
