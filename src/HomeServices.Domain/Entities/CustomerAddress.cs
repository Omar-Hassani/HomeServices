using HomeServices.Domain.Common;

namespace HomeServices.Domain.Entities;

public class CustomerAddress : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int AreaId { get; set; }
    public Area Area { get; set; } = null!;

    public string Label { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string? Building { get; set; }
    public string? Details { get; set; }
    public bool IsDefault { get; set; }
}
