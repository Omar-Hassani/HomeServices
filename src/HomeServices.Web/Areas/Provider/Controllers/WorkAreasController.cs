using HomeServices.Application.Interfaces;
using HomeServices.Web.Areas.Provider.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Provider.Controllers;

public class WorkAreasController(IProviderProfileService profileService, ILocationService locationService) : ProviderAreaController
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) => View(new WorkAreasViewModel
    {
        Cities = await locationService.GetCitiesWithAreasAsync(cancellationToken),
        SelectedAreaIds = await profileService.GetAreaIdsAsync(CurrentUserId, cancellationToken)
    });

    [HttpPost]
    public async Task<IActionResult> Index(int[]? areaIds, CancellationToken cancellationToken)
    {
        var result = await profileService.UpdateAreasAsync(CurrentUserId, areaIds ?? [], cancellationToken);
        Notify(result, "تم حفظ مناطق عملك.");
        return RedirectToAction(nameof(Index));
    }
}
