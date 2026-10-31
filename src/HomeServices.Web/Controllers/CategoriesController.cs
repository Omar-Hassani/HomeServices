using HomeServices.Application.DTOs.Providers;
using HomeServices.Application.Interfaces;
using HomeServices.Web.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Controllers;

[Route("Categories")]
public class CategoriesController(
    ICategoryService categoryService,
    IProviderDirectoryService providerDirectory,
    ILocationService locationService) : Controller
{
    private const int ProvidersPreviewCount = 8;

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetActiveCategoriesAsync(cancellationToken);
        return View(categories);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Details(string slug, CancellationToken cancellationToken)
    {
        var category = await categoryService.GetBySlugAsync(slug, cancellationToken);
        if (category is null)
        {
            return NotFound();
        }

        var providers = await providerDirectory.SearchAsync(
            new ProviderSearchCriteria { CategoryId = category.Id, PageSize = ProvidersPreviewCount },
            cancellationToken);

        var cities = await locationService.GetCitiesWithAreasAsync(cancellationToken);

        return View(new CategoryDetailsViewModel
        {
            Category = category,
            Providers = providers,
            Cities = cities
        });
    }
}
