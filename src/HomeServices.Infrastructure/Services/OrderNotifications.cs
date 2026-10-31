using HomeServices.Application.DTOs.Notifications;
using HomeServices.Domain.Enums;

namespace HomeServices.Infrastructure.Services;

/// <summary>
/// صياغة إشعارات الطلبات: من يستلم الإشعار ونصه ورابطه لكل حدث.
/// </summary>
internal static class OrderNotifications
{
    public sealed record OrderInfo(
        int Id,
        string ServiceName,
        string CustomerId,
        string CustomerName,
        string ProviderUserId,
        string ProviderName,
        DateOnly ScheduledDate);

    // الصياغة بالمبني للمجهول أو بـ "من" حتى تصح لغوياً مهما كان جنس العميل أو مقدم الخدمة
    public static NotificationMessage NewOrder(OrderInfo o) => new(
        o.ProviderUserId,
        NotificationType.NewOrder,
        "طلب خدمة جديد",
        $"طلب جديد من {FirstName(o.CustomerName)} لخدمة «{o.ServiceName}» بتاريخ {o.ScheduledDate:d MMMM}. راجع الطلب وردّ عليه.",
        NotificationLinks.ProviderOrder(o.Id));

    public static NotificationMessage StatusChanged(OrderInfo o, OrderAction action, string? note) => action switch
    {
        OrderAction.Accept => ToCustomer(o, "تم قبول طلبك",
            $"تم قبول طلبك لخدمة «{o.ServiceName}» من {o.ProviderName}. يمكنك الآن رؤية رقم التواصل في صفحة الطلب."),
        OrderAction.Reject => ToCustomer(o, "تعذّر قبول طلبك",
            $"تعذّر على {o.ProviderName} قبول طلب «{o.ServiceName}»{Reason(note)}"),
        OrderAction.Start => ToCustomer(o, "بدأ تنفيذ طلبك",
            $"بدأ العمل على طلب «{o.ServiceName}» من {o.ProviderName}."),
        OrderAction.Complete => ToCustomer(o, "اكتمل طلبك، قيّم تجربتك",
            $"تم إتمام طلب «{o.ServiceName}» من {o.ProviderName}. شاركنا تقييمك ليساعد غيرك على الاختيار."),
        OrderAction.CancelByProvider => ToCustomer(o, "تم إلغاء طلبك",
            $"تم إلغاء طلب «{o.ServiceName}» من قبل {o.ProviderName}{Reason(note)}"),
        OrderAction.CancelByCustomer => new NotificationMessage(
            o.ProviderUserId,
            NotificationType.OrderStatusChanged,
            "تم إلغاء طلب من العميل",
            $"تم إلغاء طلب «{o.ServiceName}» من قبل {FirstName(o.CustomerName)}{Reason(note)}",
            NotificationLinks.ProviderOrder(o.Id)),
        _ => throw new ArgumentOutOfRangeException(nameof(action))
    };

    private static NotificationMessage ToCustomer(OrderInfo o, string title, string message) =>
        new(o.CustomerId, NotificationType.OrderStatusChanged, title, message, NotificationLinks.CustomerOrder(o.Id));

    private static string Reason(string? note) => string.IsNullOrWhiteSpace(note) ? "." : $". السبب: {note}";

    private static string FirstName(string fullName) =>
        fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? fullName;
}
