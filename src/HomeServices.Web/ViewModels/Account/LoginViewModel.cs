using System.ComponentModel.DataAnnotations;

namespace HomeServices.Web.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "يرجى إدخال البريد الإلكتروني.")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح.")]
    [Display(Name = "البريد الإلكتروني")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "يرجى إدخال كلمة المرور.")]
    [DataType(DataType.Password)]
    [Display(Name = "كلمة المرور")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "تذكرني على هذا الجهاز")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
