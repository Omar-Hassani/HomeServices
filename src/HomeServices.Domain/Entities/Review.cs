using HomeServices.Domain.Common;

namespace HomeServices.Domain.Entities;

public class Review : AuditableEntity
{
    public int ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; } = null!;

    public string CustomerId { get; set; } = string.Empty;
    public ApplicationUser Customer { get; set; } = null!;

    public int ProviderProfileId { get; set; }
    public ProviderProfile Provider { get; set; } = null!;

    public byte Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsHidden { get; set; }
}
