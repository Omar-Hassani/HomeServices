using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Orders;
using HomeServices.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.ViewModels.Orders;

/// <summary>
/// بناء نماذج عرض الطلبات المشتركة بين لوحة العميل ولوحة مقدم الخدمة.
/// </summary>
public static class OrderViewModelFactory
{
    public static OrdersPageViewModel BuildList(
        IUrlHelper url, PagedResult<OrderSummaryDto> orders, OrderCountsDto counts, OrderStatusGroup group, bool isProviderView) => new()
    {
        Orders = orders,
        Counts = counts,
        Group = group,
        IsProviderView = isProviderView,
        TabUrl = g => url.Action("Index", new { group = g == OrderStatusGroup.Active ? (OrderStatusGroup?)null : g })!,
        DetailsUrl = id => url.Action("Details", new { id })!,
        Pagination = new PaginationViewModel
        {
            Page = orders.Page,
            TotalPages = orders.TotalPages,
            PageUrl = page => url.Action("Index", new
            {
                group = group == OrderStatusGroup.Active ? (OrderStatusGroup?)null : group,
                page = page > 1 ? page : (int?)null
            })!
        }
    };

    public static OrderPageViewModel BuildDetails(IUrlHelper url, OrderDetailsDto order, bool isNew = false) => new()
    {
        Order = order,
        ActionUrl = url.Action("Action", new { id = order.Id })!,
        BackUrl = url.Action("Index")!,
        IsNew = isNew
    };
}
