using System.Security.Claims;
using HomeServices.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.ViewComponents;

/// <summary>
/// القائمة الجانبية للوحة مقدم الخدمة، مع حالة التوثيق ونسبة اكتمال الملف.
/// </summary>
public class ProviderNavViewComponent(IProviderProfileService profileService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        var info = userId is null ? null : await profileService.GetNavInfoAsync(userId, HttpContext.RequestAborted);
        return View(info);
    }
}
