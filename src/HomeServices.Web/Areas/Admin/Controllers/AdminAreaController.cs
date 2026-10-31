using System.Security.Claims;
using HomeServices.Application.Common;
using HomeServices.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Admin.Controllers;

/// <summary>
/// أساس كل صفحات لوحة الإدارة: محمية بدور Admin.
/// </summary>
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public abstract class AdminAreaController : Controller
{
    protected string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    protected void Notify(Result result, string successMessage) =>
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? successMessage : string.Join(" ", result.Errors);

    /// <summary>
    /// العودة للصفحة السابقة (مع فلاترها) إن كانت محلية، وإلا لصفحة افتراضية.
    /// </summary>
    protected IActionResult RedirectBack(string? returnUrl, string fallbackAction) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction(fallbackAction);
}
