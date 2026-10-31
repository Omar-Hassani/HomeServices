using HomeServices.Application.Interfaces;
using HomeServices.Web.Areas.Provider.Models;
using HomeServices.Web.Extensions;
using HomeServices.Web.Security;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Provider.Controllers;

public class PortfolioController(IProviderProfileService profileService) : ProviderAreaController
{
    private const int MaxCaptionLength = 150;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) => View(new PortfolioViewModel
    {
        Images = await profileService.GetPortfolioAsync(CurrentUserId, cancellationToken),
        MaxImages = IProviderProfileService.MaxPortfolioImages
    });

    [HttpPost]
    [RequestSizeLimit(IFileStorageService.MaxImageBytes + 1024 * 1024)]
    [EnableRateLimiting(RateLimitPolicies.Uploads)]
    public async Task<IActionResult> Upload(IFormFile? image, string? caption, CancellationToken cancellationToken)
    {
        if (image is null || image.Length == 0)
        {
            TempData["Error"] = "يرجى اختيار صورة لرفعها.";
            return RedirectToAction(nameof(Index));
        }

        if (caption?.Length > MaxCaptionLength)
        {
            TempData["Error"] = $"يجب ألا يتجاوز وصف الصورة {MaxCaptionLength} حرفاً.";
            return RedirectToAction(nameof(Index));
        }

        var result = await profileService.AddPortfolioImageAsync(CurrentUserId, image.ToFileUpload(), caption, cancellationToken);
        Notify(result, "تمت إضافة الصورة إلى معرض أعمالك.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id, CancellationToken cancellationToken)
    {
        var result = await profileService.RemovePortfolioImageAsync(CurrentUserId, id, cancellationToken);
        Notify(result, "تم حذف الصورة.");
        return RedirectToAction(nameof(Index));
    }
}
