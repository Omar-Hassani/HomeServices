using System.Security.Claims;
using HomeServices.Application.Constants;
using HomeServices.Application.DTOs.Orders;
using HomeServices.Application.DTOs.Reviews;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Enums;
using HomeServices.Web.ViewModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = Roles.Customer)]
public class OrdersController(IOrderService orderService, IReviewService reviewService) : Controller
{
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> Index(OrderStatusGroup group = OrderStatusGroup.Active, int page = 1, CancellationToken cancellationToken = default)
    {
        var orders = await orderService.GetCustomerOrdersAsync(CurrentUserId, group, page, cancellationToken);
        var counts = await orderService.GetCustomerCountsAsync(CurrentUserId, cancellationToken);
        return View(OrderViewModelFactory.BuildList(Url, orders, counts, group, isProviderView: false));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var order = await orderService.GetDetailsAsync(id, CurrentUserId, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        var isNew = TempData["NewOrder"] is true;
        return View(OrderViewModelFactory.BuildDetails(Url, order, isNew));
    }

    [HttpPost]
    public async Task<IActionResult> Action(int id, OrderActionForm form, CancellationToken cancellationToken)
    {
        if (form.Action != OrderAction.CancelByCustomer)
        {
            return BadRequest();
        }

        var result = await orderService.ExecuteActionAsync(id, CurrentUserId, new OrderActionInput(form.Action, form.Note, null), cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "تم إلغاء الطلب." : string.Join(" ", result.Errors);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Review(int id, ReviewForm form, CancellationToken cancellationToken)
    {
        var result = await reviewService.CreateAsync(CurrentUserId, id, new ReviewInput(form.Rating, form.Comment), cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "شكراً لك! تم نشر تقييمك." : string.Join(" ", result.Errors);
        return RedirectToAction(nameof(Details), new { id });
    }
}
