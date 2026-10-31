using HomeServices.Application.DTOs.ProviderManagement;
using HomeServices.Application.Interfaces;
using HomeServices.Web.Areas.Provider.Models;
using HomeServices.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Provider.Controllers;

public class ServicesController(IProviderProfileService profileService, ICategoryService categoryService) : ProviderAreaController
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await BuildViewModelAsync(new ServiceFormModel(), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Save([Bind(Prefix = "Form")] ServiceFormModel form, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await profileService.SaveServiceAsync(
                CurrentUserId,
                new ProviderServiceInput(form.ServiceId!.Value, form.Price!.Value, form.PriceUnit, form.Notes),
                cancellationToken);

            if (result.Succeeded)
            {
                TempData["Success"] = "تم حفظ الخدمة وسعرها.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddErrors(result);
        }

        return View(nameof(Index), await BuildViewModelAsync(form, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id, CancellationToken cancellationToken)
    {
        var result = await profileService.RemoveServiceAsync(CurrentUserId, id, cancellationToken);
        Notify(result, "تم حذف الخدمة من ملفك.");
        return RedirectToAction(nameof(Index));
    }

    private async Task<ServicesViewModel> BuildViewModelAsync(ServiceFormModel form, CancellationToken cancellationToken) => new()
    {
        Services = await profileService.GetServicesAsync(CurrentUserId, cancellationToken),
        Catalog = await categoryService.GetCatalogAsync(cancellationToken),
        Form = form
    };
}
