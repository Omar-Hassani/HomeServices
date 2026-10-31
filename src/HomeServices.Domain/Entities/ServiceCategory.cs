using HomeServices.Domain.Common;

namespace HomeServices.Domain.Entities;

public class ServiceCategory : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// كلمات بديلة يستخدمها الناس في البحث (مثل: كهربجي، سمكري) مفصولة بمسافات.
    /// </summary>
    public string? SearchKeywords { get; set; }

    public string IconClass { get; set; } = "fa-solid fa-house";
    public string AccentColor { get; set; } = "#0E5E6F";
    public string? ImagePath { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public int? ParentCategoryId { get; set; }
    public ServiceCategory? ParentCategory { get; set; }
    public ICollection<ServiceCategory> SubCategories { get; set; } = new List<ServiceCategory>();

    public ICollection<Service> Services { get; set; } = new List<Service>();
}
