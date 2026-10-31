using System.Security.Claims;
using HomeServices.Application.Interfaces;
using HomeServices.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Controllers;

[Authorize]
public class NotificationsController(INotificationService notificationService) : Controller
{
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        var notifications = await notificationService.GetPageAsync(CurrentUserId, page, cancellationToken);
        var unread = await notificationService.GetUnreadCountAsync(CurrentUserId, cancellationToken);

        return View(new NotificationsPageViewModel
        {
            Notifications = notifications,
            UnreadCount = unread,
            Pagination = new PaginationViewModel
            {
                Page = notifications.Page,
                TotalPages = notifications.TotalPages,
                PageUrl = p => Url.Action(nameof(Index), new { page = p > 1 ? p : (int?)null })!
            }
        });
    }

    /// <summary>
    /// يعلّم الإشعار كمقروء ثم ينقل المستخدم لصفحة الحدث المرتبط به.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Open(int id, CancellationToken cancellationToken)
    {
        var link = await notificationService.OpenAsync(CurrentUserId, id, cancellationToken);
        return !string.IsNullOrEmpty(link) && Url.IsLocalUrl(link)
            ? LocalRedirect(link)
            : RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllRead(string? returnUrl, CancellationToken cancellationToken)
    {
        await notificationService.MarkAllAsReadAsync(CurrentUserId, cancellationToken);
        return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// عدد الإشعارات غير المقروءة لتحديث شارة الجرس دورياً دون إعادة تحميل الصفحة.
    /// </summary>
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> UnreadCount(CancellationToken cancellationToken) =>
        Json(new { count = await notificationService.GetUnreadCountAsync(CurrentUserId, cancellationToken) });
}
