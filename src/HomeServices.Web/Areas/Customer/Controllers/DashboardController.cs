using HomeServices.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Customer.Controllers;

/// <summary>
/// المدخل الافتراضي لمنطقة العميل (/Customer) يقود مباشرة إلى طلباتي.
/// </summary>
[Area("Customer")]
[Authorize(Roles = Roles.Customer)]
public class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Index() => RedirectToAction("Index", "Orders");
}
