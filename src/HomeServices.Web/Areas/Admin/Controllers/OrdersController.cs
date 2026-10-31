using HomeServices.Application.Interfaces;
using HomeServices.Domain.Enums;
using HomeServices.Web.Areas.Admin.Models;
using HomeServices.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Admin.Controllers;

public class OrdersController(IAdminOrderService adminOrderService) : AdminAreaController
{
    [HttpGet]
    public async Task<IActionResult> Index(OrderStatus? status, string? q, int page = 1, CancellationToken cancellationToken = default)
    {
        var orders = await adminOrderService.GetOrdersAsync(status, q, page, cancellationToken);
        return View(new OrdersPageViewModel
        {
            Orders = orders,
            Counts = await adminOrderService.GetCountsAsync(cancellationToken),
            Status = status,
            Query = q,
            Pagination = new PaginationViewModel
            {
                Page = orders.Page,
                TotalPages = orders.TotalPages,
                PageUrl = p => Url.Action(nameof(Index), new { status, q, page = p > 1 ? p : (int?)null })!
            }
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var order = await adminOrderService.GetDetailsAsync(id, cancellationToken);
        return order is null ? NotFound() : View(order);
    }
}
