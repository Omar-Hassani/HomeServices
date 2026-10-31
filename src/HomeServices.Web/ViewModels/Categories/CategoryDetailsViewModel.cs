using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Catalog;
using HomeServices.Application.DTOs.Locations;
using HomeServices.Application.DTOs.Providers;

namespace HomeServices.Web.ViewModels.Categories;

public class CategoryDetailsViewModel
{
    public required CategoryDetailsDto Category { get; init; }
    public required PagedResult<ProviderCardDto> Providers { get; init; }
    public required IReadOnlyList<CityDto> Cities { get; init; }
}
