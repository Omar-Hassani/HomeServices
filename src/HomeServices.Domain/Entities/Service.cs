using HomeServices.Domain.Common;

namespace HomeServices.Domain.Entities;

public class Service : AuditableEntity
{
    public int CategoryId { get; set; }
    public ServiceCategory Category { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ProviderService> ProviderServices { get; set; } = new List<ProviderService>();
}
