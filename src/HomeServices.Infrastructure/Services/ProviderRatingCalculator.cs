using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Infrastructure.Services;

/// <summary>
/// يعيد حساب متوسط التقييم وعدد التقييمات لمقدم الخدمة من التقييمات الظاهرة فقط.
/// الحساب من المصدر (بدل الزيادة التراكمية) يضمن بقاء القيم صحيحة عند إخفاء تقييم أو إظهاره.
/// </summary>
internal static class ProviderRatingCalculator
{
    public static async Task RecalculateAsync(ApplicationDbContext db, int providerProfileId, CancellationToken cancellationToken)
    {
        var stats = await db.Reviews
            .Where(r => r.ProviderProfileId == providerProfileId && !r.IsHidden)
            .GroupBy(r => r.ProviderProfileId)
            .Select(g => new { Count = g.Count(), Average = g.Average(r => (decimal)r.Rating) })
            .FirstOrDefaultAsync(cancellationToken);

        var profile = await db.ProviderProfiles.FirstAsync(p => p.Id == providerProfileId, cancellationToken);
        profile.ReviewsCount = stats?.Count ?? 0;
        profile.AverageRating = stats is null ? 0 : Math.Round(stats.Average, 2);
    }
}
