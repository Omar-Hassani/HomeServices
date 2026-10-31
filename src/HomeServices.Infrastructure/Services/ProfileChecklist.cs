using HomeServices.Application.DTOs.ProviderManagement;

namespace HomeServices.Infrastructure.Services;

/// <summary>
/// عناصر اكتمال ملف مقدم الخدمة: مصدر واحد تستخدمه لوحة مقدم الخدمة ولوحة الإدارة.
/// </summary>
internal static class ProfileChecklist
{
    public const int MinBioLength = 30;

    public static List<ProfileChecklistItem> Build(
        string headline, string? bio, int? cityId, int servicesCount, int areasCount, string? profileImagePath, int portfolioCount) =>
    [
        new("headline", "العنوان المهني", !string.IsNullOrWhiteSpace(headline), IsRequired: true),
        new("bio", $"نبذة تعريفية ({MinBioLength} حرفاً على الأقل)", (bio?.Trim().Length ?? 0) >= MinBioLength, IsRequired: true),
        new("city", "المدينة الرئيسية", cityId.HasValue, IsRequired: true),
        new("services", "خدمة واحدة على الأقل مع السعر", servicesCount > 0, IsRequired: true),
        new("areas", "منطقة عمل واحدة على الأقل", areasCount > 0, IsRequired: true),
        new("photo", "صورة شخصية واضحة", !string.IsNullOrEmpty(profileImagePath), IsRequired: false),
        new("portfolio", "صور من أعمالك السابقة", portfolioCount > 0, IsRequired: false)
    ];

    public static int CompletionPercent(IReadOnlyCollection<ProfileChecklistItem> items) =>
        items.Count == 0 ? 0 : items.Count(i => i.IsDone) * 100 / items.Count;

    public static bool IsComplete(IEnumerable<ProfileChecklistItem> items) =>
        items.Where(i => i.IsRequired).All(i => i.IsDone);
}
