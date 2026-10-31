using System.Security.Claims;
using HomeServices.Application.Common;
using HomeServices.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Provider.Controllers;

/// <summary>
/// أساس كل صفحات لوحة مقدم الخدمة: محمية بدور Provider وتعمل دائماً على ملف المستخدم الحالي.
/// </summary>
[Area("Provider")]
[Authorize(Roles = Roles.Provider)]
public abstract class ProviderAreaController : Controller
{
    protected string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    /// <summary>
    /// يعرض نتيجة العملية كتنبيه منبثق في الصفحة التالية.
    /// </summary>
    protected void Notify(Result result, string successMessage) =>
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? successMessage : string.Join(" ", result.Errors);
}
