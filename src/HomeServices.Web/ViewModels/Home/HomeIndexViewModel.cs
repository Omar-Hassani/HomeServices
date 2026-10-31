using HomeServices.Application.DTOs.Catalog;
using HomeServices.Application.DTOs.Providers;
using HomeServices.Application.DTOs.Stats;
using HomeServices.Web.ViewModels.Shared;

namespace HomeServices.Web.ViewModels.Home;

public class HomeIndexViewModel
{
    public required IReadOnlyList<CategoryDto> Categories { get; init; }
    public required IReadOnlyList<ProviderCardDto> FeaturedProviders { get; init; }
    public required PlatformStatsDto Stats { get; init; }
    public required SearchBarViewModel Search { get; init; }
}
