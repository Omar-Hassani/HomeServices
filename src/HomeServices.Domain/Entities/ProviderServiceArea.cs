namespace HomeServices.Domain.Entities;

public class ProviderServiceArea
{
    public int ProviderProfileId { get; set; }
    public ProviderProfile Provider { get; set; } = null!;

    public int AreaId { get; set; }
    public Area Area { get; set; } = null!;
}
