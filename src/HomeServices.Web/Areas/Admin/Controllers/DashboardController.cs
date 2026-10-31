using HomeServices.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Admin.Controllers;

public class DashboardController(IAdminDashboardService dashboardService) : AdminAreaController
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await dashboardService.GetStatsAsync(cancellationToken));
}
