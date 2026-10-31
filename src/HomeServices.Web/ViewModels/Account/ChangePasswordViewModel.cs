using System.ComponentModel.DataAnnotations;

namespace HomeServices.Web.ViewModels.Account;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "يرجى إدخال كلمة المرور الحالية.")]
    [DataType(DataType.Password)]
    [Display(Name = "كلمة المرور الحالية")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "يرجى إدخال كلمة المرور الجديدة.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "يجب ألا تقل كلمة المرور عن {2} أحرف.")]
    [RegularExpression(@"^(?=.*\d).+$", ErrorMessage = "يجب أن تحتوي كلمة المرور على رقم واحد على الأقل.")]
    [DataType(DataType.Password)]
    [Display(Name = "كلمة المرور الجديدة")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "يرجى تأكيد كلمة المرور الجديدة.")]
    [Compare(nameof(NewPassword), ErrorMessage = "كلمتا المرور غير متطابقتين.")]
    [DataType(DataType.Password)]
    [Display(Name = "تأكيد كلمة المرور الجديدة")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
