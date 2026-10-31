using HomeServices.Domain.Enums;

namespace HomeServices.Application.DTOs.Notifications;

/// <summary>
/// إشعار مطلوب إرساله لمستخدم واحد.
/// </summary>
public sealed record NotificationMessage(string UserId, NotificationType Type, string Title, string Message, string? Link);

public sealed record NotificationDto(int Id, NotificationType Type, string Title, string Message, bool IsRead, DateTime CreatedAt);

/// <summary>
/// روابط صفحات الوجهة للإشعارات، مركزية حتى لا تتكرر المسارات في الخدمات.
/// </summary>
public static class NotificationLinks
{
    public static string CustomerOrder(int orderId) => $"/Customer/Orders/Details/{orderId}";
    public static string ProviderOrder(int orderId) => $"/Provider/Orders/Details/{orderId}";
    public const string ProviderReviews = "/Provider/Reviews";
    public const string ProviderDashboard = "/Provider";
}
