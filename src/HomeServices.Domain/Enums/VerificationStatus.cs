using System.ComponentModel.DataAnnotations;

namespace HomeServices.Domain.Enums;

public enum VerificationStatus
{
    [Display(Name = "قيد المراجعة")] Pending = 0,
    [Display(Name = "موثّق")] Approved = 1,
    [Display(Name = "مرفوض")] Rejected = 2,
    [Display(Name = "موقوف")] Suspended = 3
}
