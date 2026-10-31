using System.ComponentModel.DataAnnotations;
using HomeServices.Application.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HomeServices.Web.Validation;

/// <summary>
/// يتحقق من رقم جوال سوري على الخادم، ويضيف تحققاً تقريبياً على المتصفح (أرقام ومسافات و+ فقط).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SyrianPhoneAttribute : ValidationAttribute, IClientModelValidator
{
    private const string ClientPattern = @"^[0-9+\s\-]{9,18}$";

    public SyrianPhoneAttribute()
        : base("رقم الهاتف غير صالح. أدخل رقم جوال سوري مثل 0912345678.")
    {
    }

    public override bool IsValid(object? value) =>
        value is null || (value is string text && (string.IsNullOrWhiteSpace(text) || SyrianPhoneNumber.TryNormalize(text, out _)));

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-regex", ErrorMessageString);
        context.Attributes.TryAdd("data-val-regex-pattern", ClientPattern);
    }
}
