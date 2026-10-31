using HomeServices.Application.DTOs.Providers;
using Microsoft.AspNetCore.Routing;

namespace HomeServices.Web.ViewModels.Providers;

/// <summary>
/// معاملات البحث كما تصل في رابط الصفحة (GET). القيم غير المنطقية تُصحَّح بدل رفض الطلب.
/// </summary>
public class ProviderSearchQuery
{
    public const int PageSize = 12;

    public string? Q { get; set; }
    public int? CategoryId { get; set; }
    public int? ServiceId { get; set; }
    public int? CityId { get; set; }
    public int? AreaId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinRating { get; set; }
    public bool Available { get; set; }
    public ProviderSortOrder Sort { get; set; } = ProviderSortOrder.Recommended;
    public int Page { get; set; } = 1;

    public string? NormalizedQuery => string.IsNullOrWhiteSpace(Q) ? null : Q.Trim()[..Math.Min(Q.Trim().Length, 100)];

    public bool HasFilters =>
        NormalizedQuery is not null || CategoryId.HasValue || ServiceId.HasValue || CityId.HasValue || AreaId.HasValue ||
        MinPrice.HasValue || MaxPrice.HasValue || MinRating.HasValue || Available;

    public ProviderSearchCriteria ToCriteria()
    {
        var minPrice = MinPrice is > 0 ? MinPrice : null;
        var maxPrice = MaxPrice is > 0 ? MaxPrice : null;
        if (minPrice > maxPrice)
        {
            (minPrice, maxPrice) = (maxPrice, minPrice);
        }

        return new ProviderSearchCriteria
        {
            Query = NormalizedQuery,
            CategoryId = CategoryId,
            ServiceId = ServiceId,
            CityId = CityId,
            AreaId = AreaId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            MinRating = MinRating is >= 1 and <= 5 ? MinRating : null,
            AvailableOnly = Available,
            Sort = Enum.IsDefined(Sort) ? Sort : ProviderSortOrder.Recommended,
            Page = Math.Max(1, Page),
            PageSize = PageSize
        };
    }

    /// <summary>
    /// قيم الرابط الحالية بدون الصفحة، مع إمكانية استثناء مفاتيح (لإزالة فلتر).
    /// </summary>
    public RouteValueDictionary ToRouteValues(params string[] exclude)
    {
        var values = new RouteValueDictionary();

        void Add(string key, object? value)
        {
            if (value is not null && !exclude.Contains(key, StringComparer.OrdinalIgnoreCase))
            {
                values[key] = value;
            }
        }

        Add("q", NormalizedQuery);
        Add("categoryId", CategoryId);
        Add("serviceId", ServiceId);
        Add("cityId", CityId);
        Add("areaId", AreaId);
        Add("minPrice", MinPrice);
        Add("maxPrice", MaxPrice);
        Add("minRating", MinRating);
        Add("available", Available ? true : null);
        Add("sort", Sort == ProviderSortOrder.Recommended ? null : Sort);
        return values;
    }
}
