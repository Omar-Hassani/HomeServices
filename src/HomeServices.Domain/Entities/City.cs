using HomeServices.Domain.Common;

namespace HomeServices.Domain.Entities;

public class City : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Area> Areas { get; set; } = new List<Area>();
}
