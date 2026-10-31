using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Catalog;
using HomeServices.Application.DTOs.Locations;
using HomeServices.Application.DTOs.Providers;
using HomeServices.Web.ViewModels.Shared;

namespace HomeServices.Web.ViewModels.Providers;

public class ProvidersIndexViewModel
{
    public required ProviderSearchQuery Query { get; init; }
    public required PagedResult<ProviderCardDto> Results { get; init; }
    public required IReadOnlyList<CategoryDetailsDto> Catalog { get; init; }
    public required IReadOnlyList<CityDto> Cities { get; init; }
    public required IReadOnlyList<ActiveFilter> ActiveFilters { get; init; }
    public required PaginationViewModel Pagination { get; init; }
    public required string ClearFiltersUrl { get; init; }
    public required string Heading { get; init; }
}

public sealed record ActiveFilter(string Label, string RemoveUrl);
