using HomeServices.Application.DTOs.Admin;
using HomeServices.Application.Interfaces;
using HomeServices.Web.Areas.Admin.Models;
using HomeServices.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Admin.Controllers;

public class CatalogController(ICatalogAdminService catalogService) : AdminAreaController
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await catalogService.GetCategoriesAsync(cancellationToken));

    [HttpGet]
    public IActionResult Create() => View("Edit", new CategoryEditViewModel { Form = new CategoryFormModel() });

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var category = await catalogService.GetCategoryAsync(id, cancellationToken);
        if (category is null)
        {
            return NotFound();
        }

        return View(new CategoryEditViewModel
        {
            Form = new CategoryFormModel
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                SearchKeywords = category.SearchKeywords,
                IconClass = category.IconClass,
                AccentColor = category.AccentColor,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive
            },
            Services = category.Services
        });
    }

    [HttpPost]
    public async Task<IActionResult> Save([Bind(Prefix = "Form")] CategoryFormModel form, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            if (form.Id is int id)
            {
                var result = await catalogService.UpdateCategoryAsync(id, form.ToInput(), cancellationToken);
                if (result.Succeeded)
                {
                    TempData["Success"] = "تم حفظ الفئة.";
                    return RedirectToAction(nameof(Edit), new { id });
                }

                ModelState.AddErrors(result);
            }
            else
            {
                var created = await catalogService.CreateCategoryAsync(form.ToInput(), cancellationToken);
                if (created.Succeeded)
                {
                    TempData["Success"] = "تمت إضافة الفئة. يمكنك الآن إضافة خدماتها.";
                    return RedirectToAction(nameof(Edit), new { id = created.Value });
                }

                foreach (var error in created.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
        }

        var services = form.Id is int existingId
            ? (await catalogService.GetCategoryAsync(existingId, cancellationToken))?.Services ?? []
            : [];

        return View("Edit", new CategoryEditViewModel { Form = form, Services = services });
    }

    [HttpPost]
    public async Task<IActionResult> SaveService(ServiceFormModel form, CancellationToken cancellationToken)
    {
        var result = await catalogService.SaveServiceAsync(
            form.ServiceId,
            new ServiceInput(form.CategoryId, form.Name, form.Slug, form.DisplayOrder, form.IsActive),
            cancellationToken);

        Notify(result, form.ServiceId is null ? "تمت إضافة الخدمة." : "تم حفظ الخدمة.");
        return RedirectToAction(nameof(Edit), new { id = form.CategoryId });
    }
}
