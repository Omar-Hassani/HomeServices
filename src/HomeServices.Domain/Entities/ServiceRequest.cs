using HomeServices.Domain.Common;
using HomeServices.Domain.Enums;

namespace HomeServices.Domain.Entities;

/// <summary>
/// طلب خدمة من عميل إلى مقدم خدمة.
/// </summary>
public class ServiceRequest : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;
    public ApplicationUser Customer { get; set; } = null!;

    public int ProviderProfileId { get; set; }
    public ProviderProfile Provider { get; set; } = null!;

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public int AreaId { get; set; }
    public Area Area { get; set; } = null!;

    public string AddressDetails { get; set; } = string.Empty;
    public DateOnly ScheduledDate { get; set; }
    public TimeSlot TimeSlot { get; set; }
    public string Description { get; set; } = string.Empty;

    public decimal? AgreedPrice { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? CancellationReason { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Review? Review { get; set; }
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
}
