using System.ComponentModel.DataAnnotations;

namespace HomeServices.Application.DTOs.Providers;

public enum ProviderSortOrder
{
    [Display(Name = "الأنسب")] Recommended = 0,
    [Display(Name = "الأعلى تقييماً")] HighestRated = 1,
    [Display(Name = "الأقل سعراً")] LowestPrice = 2,
    [Display(Name = "الأعلى سعراً")] HighestPrice = 3,
    [Display(Name = "الأكثر خبرة")] MostExperienced = 4,
    [Display(Name = "الأحدث انضماماً")] Newest = 5
}

public sealed record ProviderSearchCriteria
{
    public string? Query { get; init; }
    public int? CategoryId { get; init; }
    public int? ServiceId { get; init; }
    public int? CityId { get; init; }
    public int? AreaId { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public decimal? MinRating { get; init; }
    public bool AvailableOnly { get; init; }
    public ProviderSortOrder Sort { get; init; } = ProviderSortOrder.Recommended;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
}
