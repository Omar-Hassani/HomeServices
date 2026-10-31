using System.ComponentModel.DataAnnotations;
using HomeServices.Web.Validation;

namespace HomeServices.Web.ViewModels.Account;

public class ProfileViewModel
{
    [Required(ErrorMessage = "يرجى إدخال الاسم الكامل.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "يجب أن يكون الاسم بين {2} و{1} حرفاً.")]
    [Display(Name = "الاسم الكامل")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "يرجى إدخال رقم الجوال.")]
    [SyrianPhone]
    [Display(Name = "رقم الجوال")]
    public string PhoneNumber { get; set; } = string.Empty;

    // للعرض فقط ولا تُقرأ من النموذج المرسل
    public string Email { get; set; } = string.Empty;
    public string? ProfileImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];
}
