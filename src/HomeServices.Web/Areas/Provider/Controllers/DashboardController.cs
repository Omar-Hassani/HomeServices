using HomeServices.Application.Interfaces;
using HomeServices.Web.Areas.Provider.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Provider.Controllers;

public class DashboardController(IProviderProfileService profileService) : ProviderAreaController
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var dashboard = await profileService.GetDashboardAsync(CurrentUserId, cancellationToken);
        if (dashboard is null)
        {
            return NotFound();
        }

        var links = new Dictionary<string, string>
        {
            ["headline"] = Url.Action("Edit", "Profile")!,
            ["bio"] = Url.Action("Edit", "Profile")!,
            ["city"] = Url.Action("Edit", "Profile")!,
            ["services"] = Url.Action("Index", "Services")!,
            ["areas"] = Url.Action("Index", "WorkAreas")!,
            ["photo"] = Url.Action("Profile", "Account", new { area = "" })!,
            ["portfolio"] = Url.Action("Index", "Portfolio")!
        };

        return View(new DashboardViewModel { Dashboard = dashboard, ChecklistLinks = links });
    }

    [HttpPost]
    public async Task<IActionResult> SetAvailability(bool isAvailable, CancellationToken cancellationToken)
    {
        var result = await profileService.SetAvailabilityAsync(CurrentUserId, isAvailable, cancellationToken);
        Notify(result, isAvailable ? "أنت الآن متاح لاستقبال طلبات جديدة." : "تم إيقاف استقبال الطلبات الجديدة مؤقتاً.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Resubmit(CancellationToken cancellationToken)
    {
        var result = await profileService.ResubmitForReviewAsync(CurrentUserId, cancellationToken);
        Notify(result, "تم إرسال ملفك للمراجعة مجدداً. سنُعلمك فور اتخاذ القرار.");
        return RedirectToAction(nameof(Index));
    }
}
