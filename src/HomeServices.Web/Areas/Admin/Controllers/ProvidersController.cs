using HomeServices.Application.Interfaces;
using HomeServices.Domain.Enums;
using HomeServices.Web.Areas.Admin.Models;
using HomeServices.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Admin.Controllers;

public class ProvidersController(IProviderVerificationService verificationService) : AdminAreaController
{
    [HttpGet]
    public async Task<IActionResult> Index(VerificationStatus? status, string? q, int page = 1, CancellationToken cancellationToken = default)
    {
        var providers = await verificationService.GetProvidersAsync(status, q, page, cancellationToken);
        return View(new ProvidersPageViewModel
        {
            Providers = providers,
            Status = status,
            Query = q,
            Pagination = new PaginationViewModel
            {
                Page = providers.Page,
                TotalPages = providers.TotalPages,
                PageUrl = p => Url.Action(nameof(Index), new { status, q, page = p > 1 ? p : (int?)null })!
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id, string? note, string? returnUrl, CancellationToken cancellationToken)
    {
        Notify(await verificationService.ApproveAsync(id, note, cancellationToken), "تم توثيق الملف وإشعار مقدم الخدمة.");
        return RedirectBack(returnUrl, nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id, string note, string? returnUrl, CancellationToken cancellationToken)
    {
        Notify(await verificationService.RejectAsync(id, note, cancellationToken), "تم رفض الملف وإرسال الملاحظة لمقدم الخدمة.");
        return RedirectBack(returnUrl, nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Suspend(int id, string note, string? returnUrl, CancellationToken cancellationToken)
    {
        Notify(await verificationService.SuspendAsync(id, note, cancellationToken), "تم إيقاف الملف وإشعار مقدم الخدمة.");
        return RedirectBack(returnUrl, nameof(Index));
    }
}
