using HomeServices.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Admin.Controllers;

public class LocationsController(ICatalogAdminService catalogService) : AdminAreaController
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await catalogService.GetLocationsAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> AddArea(int cityId, string name, CancellationToken cancellationToken)
    {
        Notify(await catalogService.AddAreaAsync(cityId, name, cancellationToken), "تمت إضافة المنطقة.");
        return RedirectToAction(nameof(Index), null, $"city-{cityId}");
    }

    [HttpPost]
    public async Task<IActionResult> SetAreaActive(int id, int cityId, bool isActive, CancellationToken cancellationToken)
    {
        Notify(await catalogService.SetAreaActiveAsync(id, isActive, cancellationToken), isActive ? "تم تفعيل المنطقة." : "تم تعطيل المنطقة.");
        return RedirectToAction(nameof(Index), null, $"city-{cityId}");
    }
}
