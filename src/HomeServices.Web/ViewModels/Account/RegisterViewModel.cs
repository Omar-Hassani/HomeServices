using System.ComponentModel.DataAnnotations;
using HomeServices.Application.DTOs.Account;
using HomeServices.Web.Validation;

namespace HomeServices.Web.ViewModels.Account;

public class RegisterViewModel
{
    [Display(Name = "نوع الحساب")]
    public AccountType AccountType { get; set; } = AccountType.Customer;

    [Required(ErrorMessage = "يرجى إدخال الاسم الكامل.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "يجب أن يكون الاسم بين {2} و{1} حرفاً.")]
    [Display(Name = "الاسم الكامل")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "يرجى إدخال البريد الإلكتروني.")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح.")]
    [StringLength(256, ErrorMessage = "البريد الإلكتروني طويل جداً.")]
    [Display(Name = "البريد الإلكتروني")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "يرجى إدخال رقم الجوال.")]
    [SyrianPhone]
    [Display(Name = "رقم الجوال")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "يرجى إدخال كلمة المرور.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "يجب ألا تقل كلمة المرور عن {2} أحرف.")]
    [RegularExpression(@"^(?=.*\d).+$", ErrorMessage = "يجب أن تحتوي كلمة المرور على رقم واحد على الأقل.")]
    [DataType(DataType.Password)]
    [Display(Name = "كلمة المرور")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "يرجى تأكيد كلمة المرور.")]
    [Compare(nameof(Password), ErrorMessage = "كلمتا المرور غير متطابقتين.")]
    [DataType(DataType.Password)]
    [Display(Name = "تأكيد كلمة المرور")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
