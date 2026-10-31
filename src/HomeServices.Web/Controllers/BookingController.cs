using System.Security.Claims;
using HomeServices.Application.Common;
using HomeServices.Application.Constants;
using HomeServices.Application.DTOs.Orders;
using HomeServices.Application.Interfaces;
using HomeServices.Web.Extensions;
using HomeServices.Web.ViewModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Controllers;

[Authorize(Roles = Roles.Customer)]
public class BookingController(IOrderService orderService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Create(int providerId, int? serviceId, CancellationToken cancellationToken)
    {
        var data = await orderService.GetBookingFormAsync(providerId, cancellationToken);
        if (data is null)
        {
            return NotFound();
        }

        var model = new BookingFormViewModel
        {
            ProviderId = providerId,
            ServiceId = data.Services.Any(s => s.ServiceId == serviceId) ? serviceId
                : data.Services.Count == 1 ? data.Services[0].ServiceId : null,
            AreaId = data.Areas.Sum(c => c.Areas.Count) == 1 ? data.Areas[0].Areas[0].Id : null
        };

        return View(Populate(model, data));
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookingFormViewModel model, CancellationToken cancellationToken)
    {
        var data = await orderService.GetBookingFormAsync(model.ProviderId, cancellationToken);
        if (data is null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var result = await orderService.CreateAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                new CreateOrderInput(model.ProviderId, model.ServiceId!.Value, model.AreaId!.Value, model.AddressDetails,
                    model.ScheduledDate!.Value, model.TimeSlot, model.Description),
                cancellationToken);

            if (result.Succeeded)
            {
                TempData["Success"] = $"تم إرسال طلبك بنجاح! رقم الطلب: {result.Value!.OrderNumber}";
                TempData["NewOrder"] = true;
                return RedirectToAction("Details", "Orders", new { area = "Customer", id = result.Value.Id });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        return View(Populate(model, data));
    }

    private static BookingFormViewModel Populate(BookingFormViewModel model, BookingFormDataDto data)
    {
        model.Data = data;
        model.MinDate = SyriaTime.Today;
        model.MaxDate = SyriaTime.Today.AddDays(IOrderService.MaxBookingDaysAhead);
        return model;
    }
}
