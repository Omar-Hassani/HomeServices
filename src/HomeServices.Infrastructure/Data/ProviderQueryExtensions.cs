using HomeServices.Domain.Entities;
using HomeServices.Domain.Enums;

namespace HomeServices.Infrastructure.Data;

internal static class ProviderQueryExtensions
{
    /// <summary>
    /// مقدمو الخدمات الذين يحق لهم الظهور للعموم: ملف موثّق وحساب فعّال.
    /// </summary>
    public static IQueryable<ProviderProfile> WherePubliclyVisible(this IQueryable<ProviderProfile> query) =>
        query.Where(p => p.VerificationStatus == VerificationStatus.Approved && p.User.IsActive);
}
