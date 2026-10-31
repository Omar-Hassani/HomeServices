namespace HomeServices.Web.ViewModels.Shared;

public class RatingSummaryViewModel
{
    public required decimal AverageRating { get; init; }
    public required int ReviewsCount { get; init; }

    /// <summary>
    /// عدد التقييمات لكل نجمة: الفهرس 0 لنجمة واحدة ... الفهرس 4 لخمس نجوم.
    /// </summary>
    public required IReadOnlyList<int> Breakdown { get; init; }

    public int Total => Breakdown.Sum();

    public int Percentage(int stars) => Total == 0 ? 0 : (int)Math.Round(Breakdown[stars - 1] * 100.0 / Total);
}
