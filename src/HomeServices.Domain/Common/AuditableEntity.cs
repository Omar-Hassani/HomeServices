namespace HomeServices.Domain.Common;

/// <summary>
/// كيان يحمل تواريخ الإنشاء والتعديل، وتُضبط تلقائياً عند الحفظ في ApplicationDbContext.
/// </summary>
public abstract class AuditableEntity : BaseEntity, IAuditable
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
