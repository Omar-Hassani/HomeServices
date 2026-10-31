using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Notifications;

namespace HomeServices.Web.ViewModels.Shared;

public class NotificationsPageViewModel
{
    public required PagedResult<NotificationDto> Notifications { get; init; }
    public required int UnreadCount { get; init; }
    public required PaginationViewModel Pagination { get; init; }
}

public class NotificationBellViewModel
{
    public required int UnreadCount { get; init; }
    public required IReadOnlyList<NotificationDto> Recent { get; init; }
}
