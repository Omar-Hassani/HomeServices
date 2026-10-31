using HomeServices.Domain.Common;
using HomeServices.Domain.Enums;

namespace HomeServices.Domain.Entities;

/// <summary>
/// خدمة يقدمها مقدم خدمة معيّن مع سعره الخاص.
/// </summary>
public class ProviderService : BaseEntity
{
    public int ProviderProfileId { get; set; }
    public ProviderProfile Provider { get; set; } = null!;

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public decimal Price { get; set; }
    public PriceUnit PriceUnit { get; set; } = PriceUnit.PerVisit;
    public string? Notes { get; set; }
}
