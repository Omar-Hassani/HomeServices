using System.ComponentModel.DataAnnotations;

namespace HomeServices.Domain.Enums;

/// <summary>
/// الإجراءات الممكنة على طلب الخدمة. من يملك كل إجراء ومتى يُسمح به يحدده ServiceRequestWorkflow.
/// </summary>
public enum OrderAction
{
    [Display(Name = "قبول الطلب")] Accept = 0,
    [Display(Name = "رفض الطلب")] Reject = 1,
    [Display(Name = "بدء التنفيذ")] Start = 2,
    [Display(Name = "إتمام الطلب")] Complete = 3,
    [Display(Name = "إلغاء الطلب")] CancelByCustomer = 4,
    [Display(Name = "إلغاء الطلب")] CancelByProvider = 5
}
