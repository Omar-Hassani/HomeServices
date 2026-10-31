using HomeServices.Application.DTOs.Providers;

namespace HomeServices.Web.ViewModels.Providers;

public class ProviderDetailsViewModel
{
    public required ProviderDetailsDto Provider { get; init; }

    /// <summary>
    /// الخدمات مجمّعة حسب الفئة للعرض.
    /// </summary>
    public IEnumerable<IGrouping<string, ProviderServiceItemDto>> ServicesByCategory =>
        Provider.Services.GroupBy(s => s.CategoryName);

    public int TotalVisibleReviews => Provider.RatingBreakdown.Sum();

    public int RatingPercentage(int stars) =>
        TotalVisibleReviews == 0 ? 0 : (int)Math.Round(Provider.RatingBreakdown[stars - 1] * 100.0 / TotalVisibleReviews);
}
