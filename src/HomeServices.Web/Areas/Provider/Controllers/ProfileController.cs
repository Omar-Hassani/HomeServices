using HomeServices.Application.DTOs.ProviderManagement;
using HomeServices.Application.Interfaces;
using HomeServices.Web.Areas.Provider.Models;
using HomeServices.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Provider.Controllers;

public class ProfileController(IProviderProfileService profileService, ILocationService locationService) : ProviderAreaController
{
    [HttpGet]
    public async Task<IActionResult> Edit(CancellationToken cancellationToken)
    {
        var profile = await profileService.GetProfileForEditAsync(CurrentUserId, cancellationToken);
        if (profile is null)
        {
            return NotFound();
        }

        return View(new ProfileFormViewModel
        {
            Headline = profile.Headline,
            Bio = profile.Bio,
            YearsOfExperience = profile.YearsOfExperience,
            CityId = profile.CityId,
            IsAvailable = profile.IsAvailable,
            Cities = await locationService.GetCitiesWithAreasAsync(cancellationToken)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProfileFormViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await profileService.UpdateProfileAsync(
                CurrentUserId,
                new ProviderProfileEditDto(model.Headline, model.Bio, model.YearsOfExperience, model.CityId, model.IsAvailable),
                cancellationToken);

            if (result.Succeeded)
            {
                TempData["Success"] = "تم حفظ ملفك الاحترافي بنجاح.";
                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddErrors(result);
        }

        model.Cities = await locationService.GetCitiesWithAreasAsync(cancellationToken);
        return View(model);
    }
}
