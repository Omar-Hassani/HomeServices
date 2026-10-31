using System.ComponentModel.DataAnnotations;

namespace HomeServices.Domain.Enums;

public enum OrderStatus
{
    [Display(Name = "بانتظار الرد")] Pending = 0,
    [Display(Name = "مقبول")] Accepted = 1,
    [Display(Name = "قيد التنفيذ")] InProgress = 2,
    [Display(Name = "مكتمل")] Completed = 3,
    [Display(Name = "مرفوض")] Rejected = 4,
    [Display(Name = "ملغى")] Cancelled = 5
}
