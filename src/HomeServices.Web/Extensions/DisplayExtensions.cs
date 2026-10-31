using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using HomeServices.Application.Common;
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

    /// <summary>
    /// تاريخ ووقت بتوقيت سوريا، مثل: 31 تشرين الأول 2026، 14:05
    /// </summary>
    public static string ToSyriaDateTime(this DateTime utc) =>
        SyriaTime.FromUtc(utc).ToString("d MMMM yyyy، HH:mm");

    public static string ToSyriaDate(this DateTime utc) =>
        SyriaTime.FromUtc(utc).ToString("d MMMM yyyy");

    /// <summary>
    /// موعد الطلب بصيغة مقروءة: "اليوم" أو "غداً" أو اسم اليوم والتاريخ.
    /// </summary>
    public static string ToScheduleText(this DateOnly date)
    {
        var today = SyriaTime.Today;
        if (date == today) return "اليوم";
        if (date == today.AddDays(1)) return "غداً";
        return date.ToString("dddd d MMMM");
    }

    /// <summary>
    /// وقت نسبي بالعربية: "الآن"، "منذ 5 دقائق"، "منذ ساعتين"، "أمس"... وبعد أسبوع يُعرض التاريخ.
    /// </summary>
    public static string ToRelativeTime(this DateTime utc)
    {
        var elapsed = DateTime.UtcNow - DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        if (elapsed < TimeSpan.FromMinutes(1)) return "الآن";
        if (elapsed < TimeSpan.FromHours(1)) return "منذ " + ArabicCount((int)elapsed.TotalMinutes, "دقيقة", "دقيقتين", "دقائق", "دقيقة");
        if (elapsed < TimeSpan.FromDays(1)) return "منذ " + ArabicCount((int)elapsed.TotalHours, "ساعة", "ساعتين", "ساعات", "ساعة");
        if (elapsed < TimeSpan.FromDays(2)) return "أمس";
        if (elapsed < TimeSpan.FromDays(7)) return "منذ " + ArabicCount((int)elapsed.TotalDays, "يوم", "يومين", "أيام", "يوماً");
        return utc.ToSyriaDate();
    }

    public static (string Icon, string Tone) ToNotificationStyle(this NotificationType type) => type switch
    {
        NotificationType.NewOrder => ("fa-solid fa-inbox", "is-primary"),
        NotificationType.OrderStatusChanged => ("fa-solid fa-arrows-rotate", "is-info"),
        NotificationType.NewReview => ("fa-solid fa-star", "is-accent"),
        NotificationType.ProviderVerification => ("fa-solid fa-shield-halved", "is-success"),
        _ => ("fa-solid fa-bell", "is-muted")
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
