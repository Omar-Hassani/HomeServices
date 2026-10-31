using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Notifications;

namespace HomeServices.Application.Interfaces;

public interface INotificationService
{
    /// <summary>
    /// يحفظ الإشعار ثم يمرّره لقنوات الإرسال الإضافية. لا يرمي استثناءات: فشل الإشعار لا يُفشل العملية الأصلية.
    /// </summary>
    Task NotifyAsync(NotificationMessage message, CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationDto>> GetRecentAsync(string userId, int count, CancellationToken cancellationToken = default);

    Task<PagedResult<NotificationDto>> GetPageAsync(string userId, int page, CancellationToken cancellationToken = default);

    /// <summary>
    /// يعلّم الإشعار كمقروء ويُرجع رابط وجهته؛ null إن لم يكن الإشعار للمستخدم.
    /// </summary>
    Task<string?> OpenAsync(string userId, int notificationId, CancellationToken cancellationToken = default);

    Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// قناة إرسال إضافية للإشعارات (بريد إلكتروني، SMS، إشعارات فورية...).
/// تُسجَّل في حاوية الخدمات وتُستدعى بعد حفظ الإشعار في قاعدة البيانات.
/// </summary>
public interface INotificationChannel
{
    Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}
