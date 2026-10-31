using System.Security.Claims;
using HomeServices.Application.Constants;
using HomeServices.Application.DTOs.Catalog;
using HomeServices.Application.DTOs.Locations;
using HomeServices.Application.Interfaces;
using HomeServices.Web.Extensions;
using HomeServices.Web.ViewModels.Providers;
using HomeServices.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Controllers;

public class ProvidersController(
    IProviderDirectoryService providerDirectory,
    ICategoryService categoryService,
    ILocationService locationService,
    IReviewService reviewService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(ProviderSearchQuery query, CancellationToken cancellationToken)
    {
        var results = await providerDirectory.SearchAsync(query.ToCriteria(), cancellationToken);
        var catalog = await categoryService.GetCatalogAsync(cancellationToken);
        var cities = await locationService.GetCitiesWithAreasAsync(cancellationToken);

        var model = new ProvidersIndexViewModel
        {
            Query = query,
            Results = results,
            Catalog = catalog,
            Cities = cities,
            ActiveFilters = BuildActiveFilters(query, catalog, cities),
            Heading = BuildHeading(query, catalog, cities),
            ClearFiltersUrl = Url.Action(nameof(Index))!,
            Pagination = new PaginationViewModel
            {
                Page = results.Page,
                TotalPages = results.TotalPages,
                PageUrl = page =>
                {
                    var values = query.ToRouteValues();
                    if (page > 1)
                    {
                        values["page"] = page;
                    }

                    return Url.Action(nameof(Index), values)!;
                }
            }
        };

        ViewData["Title"] = model.Heading;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var provider = await providerDirectory.GetDetailsAsync(
            id,
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            User.IsInRole(Roles.Admin),
            cancellationToken);

        if (provider is null)
        {
            return NotFound();
        }

        return View(new ProviderDetailsViewModel { Provider = provider });
    }

    [HttpGet]
    public async Task<IActionResult> Reviews(int id, int page = 1, CancellationToken cancellationToken = default)
    {
        var reviews = await reviewService.GetPublicReviewsAsync(id, page, cancellationToken);
        if (reviews is null)
        {
            return NotFound();
        }

        return View(new ReviewsPageViewModel
        {
            Reviews = reviews,
            Pagination = new PaginationViewModel
            {
                Page = reviews.Reviews.Page,
                TotalPages = reviews.Reviews.TotalPages,
                PageUrl = p => Url.Action(nameof(Reviews), new { id, page = p > 1 ? p : (int?)null })!
            }
        });
    }

    private static string BuildHeading(ProviderSearchQuery query, IReadOnlyList<CategoryDetailsDto> catalog, IReadOnlyList<CityDto> cities)
    {
        var service = catalog.SelectMany(c => c.Services).FirstOrDefault(s => s.Id == query.ServiceId);
        var category = catalog.FirstOrDefault(c => c.Id == query.CategoryId);

        var heading = service is not null ? $"مقدمو خدمة {service.Name}"
            : category is not null ? $"مقدمو خدمات {category.Name}"
            : query.NormalizedQuery is not null ? $"نتائج البحث عن «{query.NormalizedQuery}»"
            : "جميع مقدمي الخدمات";

        var area = cities.SelectMany(c => c.Areas.Select(a => (City: c, Area: a))).FirstOrDefault(x => x.Area.Id == query.AreaId);
        var city = cities.FirstOrDefault(c => c.Id == query.CityId);

        if (area.Area is not null)
        {
            heading += $" في {area.Area.Name}، {area.City.Name}";
        }
        else if (city is not null)
        {
            heading += $" في {city.Name}";
        }

        return heading;
    }

    private List<ActiveFilter> BuildActiveFilters(ProviderSearchQuery query, IReadOnlyList<CategoryDetailsDto> catalog, IReadOnlyList<CityDto> cities)
    {
        var filters = new List<ActiveFilter>();
        string RemoveUrl(params string[] keys) => Url.Action(nameof(Index), query.ToRouteValues(keys))!;

        if (query.NormalizedQuery is { } text)
        {
            filters.Add(new ActiveFilter($"«{text}»", RemoveUrl("q")));
        }

        if (catalog.FirstOrDefault(c => c.Id == query.CategoryId) is { } category)
        {
            filters.Add(new ActiveFilter(category.Name, RemoveUrl("categoryId", "serviceId")));
        }

        if (catalog.SelectMany(c => c.Services).FirstOrDefault(s => s.Id == query.ServiceId) is { } service)
        {
            filters.Add(new ActiveFilter(service.Name, RemoveUrl("serviceId")));
        }

        if (cities.FirstOrDefault(c => c.Id == query.CityId) is { } city)
        {
            filters.Add(new ActiveFilter(city.Name, RemoveUrl("cityId", "areaId")));
        }

        if (cities.SelectMany(c => c.Areas).FirstOrDefault(a => a.Id == query.AreaId) is { } area)
        {
            filters.Add(new ActiveFilter(area.Name, RemoveUrl("areaId")));
        }

        var criteria = query.ToCriteria();
        if (criteria.MinPrice.HasValue || criteria.MaxPrice.HasValue)
        {
            var label = (criteria.MinPrice, criteria.MaxPrice) switch
            {
                ({ } min, { } max) => $"السعر: {min.ToPrice()} - {max.ToPrice()}",
                ({ } min, null) => $"السعر من {min.ToPrice()}",
                (null, { } max) => $"السعر حتى {max.ToPrice()}",
                _ => string.Empty
            };
            filters.Add(new ActiveFilter(label, RemoveUrl("minPrice", "maxPrice")));
        }

        if (criteria.MinRating is { } rating)
        {
            filters.Add(new ActiveFilter($"تقييم {rating.ToRatingText()}+", RemoveUrl("minRating")));
        }

        if (query.Available)
        {
            filters.Add(new ActiveFilter("متاح الآن", RemoveUrl("available")));
        }

        return filters;
    }
}
