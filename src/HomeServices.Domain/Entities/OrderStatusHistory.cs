using HomeServices.Domain.Common;
using HomeServices.Domain.Enums;

namespace HomeServices.Domain.Entities;

public class OrderStatusHistory : BaseEntity
{
    public int ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; } = null!;

    public OrderStatus? FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }

    public string? ChangedByUserId { get; set; }
    public ApplicationUser? ChangedByUser { get; set; }

    public string? Note { get; set; }
    public DateTime ChangedAt { get; set; }
}
