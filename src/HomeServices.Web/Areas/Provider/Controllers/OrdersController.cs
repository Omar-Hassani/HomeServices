using HomeServices.Application.DTOs.Orders;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Enums;
using HomeServices.Web.ViewModels.Orders;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Provider.Controllers;

public class OrdersController(IOrderService orderService) : ProviderAreaController
{
    private static readonly IReadOnlyDictionary<OrderAction, string> SuccessMessages = new Dictionary<OrderAction, string>
    {
        [OrderAction.Accept] = "تم قبول الطلب. أصبحت بيانات التواصل مع العميل متاحة لك الآن.",
        [OrderAction.Reject] = "تم رفض الطلب.",
        [OrderAction.Start] = "تم تحديث حالة الطلب إلى «قيد التنفيذ».",
        [OrderAction.Complete] = "أحسنت! تم إتمام الطلب بنجاح.",
        [OrderAction.CancelByProvider] = "تم إلغاء الطلب."
    };

    [HttpGet]
    public async Task<IActionResult> Index(OrderStatusGroup group = OrderStatusGroup.Active, int page = 1, CancellationToken cancellationToken = default)
    {
        var orders = await orderService.GetProviderOrdersAsync(CurrentUserId, group, page, cancellationToken);
        var counts = await orderService.GetProviderCountsAsync(CurrentUserId, cancellationToken);
        return View(OrderViewModelFactory.BuildList(Url, orders, counts, group, isProviderView: true));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var order = await orderService.GetDetailsAsync(id, CurrentUserId, cancellationToken);
        if (order is null || order.ViewerIsCustomer)
        {
            return NotFound();
        }

        return View(OrderViewModelFactory.BuildDetails(Url, order));
    }

    [HttpPost]
    public async Task<IActionResult> Action(int id, OrderActionForm form, CancellationToken cancellationToken)
    {
        if (form.Action == OrderAction.CancelByCustomer || !Enum.IsDefined(form.Action))
        {
            return BadRequest();
        }

        var result = await orderService.ExecuteActionAsync(id, CurrentUserId, new OrderActionInput(form.Action, form.Note, form.Price), cancellationToken);
        Notify(result, SuccessMessages[form.Action]);
        return RedirectToAction(nameof(Details), new { id });
    }
}
