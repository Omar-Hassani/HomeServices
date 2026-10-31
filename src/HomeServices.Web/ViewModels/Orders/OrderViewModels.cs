using System.ComponentModel.DataAnnotations;
using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Orders;
using HomeServices.Domain.Enums;
using HomeServices.Web.ViewModels.Shared;

namespace HomeServices.Web.ViewModels.Orders;

public class BookingFormViewModel
{
    [Required]
    public int ProviderId { get; set; }

    [Required(ErrorMessage = "يرجى اختيار الخدمة.")]
    [Display(Name = "الخدمة المطلوبة")]
    public int? ServiceId { get; set; }

    [Required(ErrorMessage = "يرجى اختيار المنطقة.")]
    [Display(Name = "المنطقة")]
    public int? AreaId { get; set; }

    [Required(ErrorMessage = "يرجى إدخال العنوان التفصيلي.")]
    [StringLength(300, MinimumLength = 10, ErrorMessage = "يجب أن يكون العنوان بين {2} و{1} حرفاً.")]
    [Display(Name = "العنوان التفصيلي")]
    public string AddressDetails { get; set; } = string.Empty;

    [Required(ErrorMessage = "يرجى اختيار تاريخ الموعد.")]
    [Display(Name = "تاريخ الموعد")]
    public DateOnly? ScheduledDate { get; set; }

    [Display(Name = "الفترة المفضلة")]
    public TimeSlot TimeSlot { get; set; } = TimeSlot.Morning;

    [Required(ErrorMessage = "يرجى وصف المطلوب.")]
    [StringLength(2000, MinimumLength = 15, ErrorMessage = "يجب أن يكون الوصف بين {2} و{1} حرفاً.")]
    [Display(Name = "وصف المشكلة أو العمل المطلوب")]
    public string Description { get; set; } = string.Empty;

    public BookingFormDataDto? Data { get; set; }
    public DateOnly MinDate { get; set; }
    public DateOnly MaxDate { get; set; }
}

public class OrdersPageViewModel
{
    public required PagedResult<OrderSummaryDto> Orders { get; init; }
    public required OrderCountsDto Counts { get; init; }
    public required OrderStatusGroup Group { get; init; }
    public required bool IsProviderView { get; init; }
    public required PaginationViewModel Pagination { get; init; }
    public required Func<OrderStatusGroup, string> TabUrl { get; init; }
    public required Func<int, string> DetailsUrl { get; init; }
}

public class OrderPageViewModel
{
    public required OrderDetailsDto Order { get; init; }
    public required string ActionUrl { get; init; }
    public required string BackUrl { get; init; }
    public bool IsNew { get; init; }
}

public class OrderActionForm
{
    public OrderAction Action { get; set; }

    [StringLength(500, ErrorMessage = "يجب ألا يتجاوز النص {1} حرف.")]
    public string? Note { get; set; }

    public decimal? Price { get; set; }
}
