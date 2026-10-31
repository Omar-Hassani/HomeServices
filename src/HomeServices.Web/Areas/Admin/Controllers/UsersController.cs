using HomeServices.Application.DTOs.Admin;
using HomeServices.Application.Interfaces;
using HomeServices.Web.Areas.Admin.Models;
using HomeServices.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Admin.Controllers;

public class UsersController(IUserAdminService userAdminService) : AdminAreaController
{
    [HttpGet]
    public async Task<IActionResult> Index(string? q, string? role, UserStatusFilter status = UserStatusFilter.All, int page = 1, CancellationToken cancellationToken = default)
    {
        var users = await userAdminService.GetUsersAsync(q, role, status, page, cancellationToken);
        var statusValue = status == UserStatusFilter.All ? (UserStatusFilter?)null : status;

        return View(new UsersPageViewModel
        {
            Users = users,
            Query = q,
            Role = role,
            Status = status,
            CurrentUserId = CurrentUserId,
            Pagination = new PaginationViewModel
            {
                Page = users.Page,
                TotalPages = users.TotalPages,
                PageUrl = p => Url.Action(nameof(Index), new { q, role, status = statusValue, page = p > 1 ? p : (int?)null })!
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> SetActive(string id, bool isActive, string? returnUrl, CancellationToken cancellationToken)
    {
        var result = await userAdminService.SetActiveAsync(CurrentUserId, id, isActive, cancellationToken);
        Notify(result, isActive ? "تم تفعيل الحساب." : "تم إيقاف الحساب وإنهاء جلساته.");
        return RedirectBack(returnUrl, nameof(Index));
    }
}
