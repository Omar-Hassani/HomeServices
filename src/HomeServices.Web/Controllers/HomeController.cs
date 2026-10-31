using System.Diagnostics;
using HomeServices.Application.Interfaces;
using HomeServices.Web.ViewModels.Home;
using HomeServices.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Controllers;

public class HomeController(
    ICategoryService categoryService,
    ILocationService locationService,
    IProviderDirectoryService providerDirectory,
    IPlatformStatsService statsService) : Controller
{
    private const int FeaturedProvidersCount = 8;
    private const int SearchSuggestionsCount = 4;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetActiveCategoriesAsync(cancellationToken);
        var cities = await locationService.GetCitiesWithAreasAsync(cancellationToken);
        var providers = await providerDirectory.GetFeaturedAsync(FeaturedProvidersCount, cancellationToken);
        var stats = await statsService.GetStatsAsync(cancellationToken);

        var model = new HomeIndexViewModel
        {
            Categories = categories,
            FeaturedProviders = providers,
            Stats = stats,
            Search = new SearchBarViewModel
            {
                Cities = cities,
                Suggestions = categories.Take(SearchSuggestionsCount).Select(c => c.Name).ToList()
            }
        };

        return View(model);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    [Route("Home/StatusCode/{code:int}")]
    public IActionResult StatusCodePage(int code)
    {
        Response.StatusCode = code;
        return View("StatusCode", new StatusCodeViewModel { Code = code });
    }
}
