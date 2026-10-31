using System.Security.Claims;
using HomeServices.Application.Interfaces;
using HomeServices.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.ViewComponents;

/// <summary>
/// جرس الإشعارات في الهيدر: عدد غير المقروء وآخر الإشعارات.
/// </summary>
public class NotificationBellViewComponent(INotificationService notificationService) : ViewComponent
{
    private const int RecentCount = 6;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Content(string.Empty);
        }

        var token = HttpContext.RequestAborted;
        return View(new NotificationBellViewModel
        {
            UnreadCount = await notificationService.GetUnreadCountAsync(userId, token),
            Recent = await notificationService.GetRecentAsync(userId, RecentCount, token)
        });
    }
}
