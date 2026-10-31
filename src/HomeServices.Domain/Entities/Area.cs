using HomeServices.Domain.Common;

namespace HomeServices.Domain.Entities;

public class Area : BaseEntity
{
    public int CityId { get; set; }
    public City City { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
