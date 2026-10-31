using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using HomeServices.Application.Constants;
using HomeServices.Domain.Enums;

namespace HomeServices.Web.Extensions;

public static class DisplayExtensions
{
    public const string CurrencySymbol = "ل.س";

    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }

    public static string ToPrice(this decimal amount) =>
        $"{amount.ToString("N0", CultureInfo.InvariantCulture)} {CurrencySymbol}";

    public static string ToPrice(this decimal amount, PriceUnit unit) => unit switch
    {
        PriceUnit.Fixed => amount.ToPrice(),
        _ => $"{amount.ToPrice()} {unit.GetDisplayName()}"
    };

    public static string ToRoleDisplayName(this string role) => role switch
    {
        Roles.Admin => "مدير المنصة",
        Roles.Provider => "مقدم خدمة",
        Roles.Customer => "عميل",
        _ => role
    };

    public static string ToRatingText(this decimal rating) =>
        rating.ToString("0.0", CultureInfo.InvariantCulture);

    public static string ToProvidersCountText(this int count) =>
        ArabicCount(count, "مقدم خدمة واحد", "مقدما خدمة", "مقدمي خدمات", "مقدم خدمة");

    public static string ToYearsOfExperienceText(this int years) =>
        years <= 0 ? "خبرة حديثة" : $"{ArabicCount(years, "سنة واحدة", "سنتان", "سنوات", "سنة")} خبرة";

    public static string ToReviewsCountText(this int count) =>
        ArabicCount(count, "تقييم واحد", "تقييمان", "تقييمات", "تقييم");

    /// <summary>
    /// صياغة العدد مع المعدود وفق قواعد العربية: 1، 2، 3-10، 11 فما فوق.
    /// </summary>
    private static string ArabicCount(int count, string one, string two, string few, string many) => count switch
    {
        1 => one,
        2 => two,
        >= 3 and <= 10 => $"{count} {few}",
        _ => $"{count.ToString("N0", CultureInfo.InvariantCulture)} {many}"
    };

    /// <summary>
    /// الحرفان الأولان من الاسم لعرضهما بدلاً من الصورة الشخصية عند غيابها.
    /// </summary>
    public static string ToInitials(this string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "؟",
            1 => parts[0][..1],
            _ => $"{parts[0][0]} {parts[^1][0]}"
        };
    }
}
