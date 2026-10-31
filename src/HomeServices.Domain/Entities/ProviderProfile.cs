using HomeServices.Domain.Common;
using HomeServices.Domain.Enums;

namespace HomeServices.Domain.Entities;

public class ProviderProfile : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string Headline { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int YearsOfExperience { get; set; }

    public int? CityId { get; set; }
    public City? City { get; set; }

    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public string? VerificationNote { get; set; }
    public bool IsAvailable { get; set; } = true;

    // قيم مجمّعة تُحدَّث عند إضافة تقييم أو إتمام طلب، لتسريع البحث والترتيب
    public decimal AverageRating { get; set; }
    public int ReviewsCount { get; set; }
    public int CompletedOrdersCount { get; set; }

    public ICollection<ProviderService> Services { get; set; } = new List<ProviderService>();
    public ICollection<ProviderServiceArea> ServiceAreas { get; set; } = new List<ProviderServiceArea>();
    public ICollection<ProviderPortfolioImage> PortfolioImages { get; set; } = new List<ProviderPortfolioImage>();
    public ICollection<ServiceRequest> Requests { get; set; } = new List<ServiceRequest>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
