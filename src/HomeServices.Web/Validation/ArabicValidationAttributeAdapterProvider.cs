using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace HomeServices.Web.Validation;

/// <summary>
/// يضيف MVC تلقائياً [Required] ضمنياً للخصائص غير القابلة لـ null (int، enum، bool) برسالة إنجليزية
/// ("The X field is required."). هذا المزوّد يعطي هذه السمات رسالة عربية افتراضية،
/// ويُستخدم لرسائل التحقق في المتصفح والخادم معاً.
/// </summary>
public sealed class ArabicValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private readonly ValidationAttributeAdapterProvider _inner = new();

    public IAttributeAdapter? GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer? stringLocalizer)
    {
        if (attribute is RequiredAttribute required && required.ErrorMessage is null && required.ErrorMessageResourceName is null)
        {
            required.ErrorMessage = "الحقل «{0}» مطلوب.";
        }

        return _inner.GetAttributeAdapter(attribute, stringLocalizer);
    }
}
