using HomeServices.Domain.Common;

namespace HomeServices.Domain.Entities;

public class ProviderPortfolioImage : BaseEntity
{
    public int ProviderProfileId { get; set; }
    public ProviderProfile Provider { get; set; } = null!;

    public string ImagePath { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
