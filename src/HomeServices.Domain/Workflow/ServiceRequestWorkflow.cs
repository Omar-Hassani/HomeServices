using HomeServices.Domain.Enums;

namespace HomeServices.Domain.Workflow;

/// <summary>
/// قواعد انتقال حالات طلب الخدمة:
///   Pending    → Accepted (مقدم الخدمة) | Rejected (مقدم الخدمة) | Cancelled (العميل)
///   Accepted   → InProgress (مقدم الخدمة) | Cancelled (العميل أو مقدم الخدمة)
///   InProgress → Completed (مقدم الخدمة)
///   Completed / Rejected / Cancelled حالات نهائية.
/// </summary>
public static class ServiceRequestWorkflow
{
    private static readonly IReadOnlyDictionary<OrderAction, (OrderStatus[] From, OrderStatus To, bool ByCustomer)> Rules =
        new Dictionary<OrderAction, (OrderStatus[], OrderStatus, bool)>
        {
            [OrderAction.Accept] = ([OrderStatus.Pending], OrderStatus.Accepted, false),
            [OrderAction.Reject] = ([OrderStatus.Pending], OrderStatus.Rejected, false),
            [OrderAction.Start] = ([OrderStatus.Accepted], OrderStatus.InProgress, false),
            [OrderAction.Complete] = ([OrderStatus.InProgress], OrderStatus.Completed, false),
            [OrderAction.CancelByCustomer] = ([OrderStatus.Pending, OrderStatus.Accepted], OrderStatus.Cancelled, true),
            [OrderAction.CancelByProvider] = ([OrderStatus.Accepted], OrderStatus.Cancelled, false)
        };

    public static readonly IReadOnlySet<OrderStatus> ActiveStatuses =
        new HashSet<OrderStatus> { OrderStatus.Pending, OrderStatus.Accepted, OrderStatus.InProgress };

    public static bool IsFinal(OrderStatus status) => !ActiveStatuses.Contains(status);

    public static bool CanPerform(OrderStatus current, OrderAction action, bool actorIsCustomer) =>
        Rules.TryGetValue(action, out var rule) && rule.ByCustomer == actorIsCustomer && rule.From.Contains(current);

    public static OrderStatus TargetStatus(OrderAction action) => Rules[action].To;

    /// <summary>
    /// الإجراءات التي يحق للمستخدم تنفيذها على الطلب في حالته الحالية.
    /// </summary>
    public static IReadOnlyList<OrderAction> AvailableActions(OrderStatus current, bool actorIsCustomer) =>
        Rules.Where(r => r.Value.ByCustomer == actorIsCustomer && r.Value.From.Contains(current))
            .Select(r => r.Key)
            .ToList();

    /// <summary>
    /// الإجراءات التي تتطلب ذكر سبب (الرفض والإلغاء من مقدم الخدمة).
    /// </summary>
    public static bool RequiresReason(OrderAction action) =>
        action is OrderAction.Reject or OrderAction.CancelByProvider;
}
