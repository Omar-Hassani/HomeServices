using HomeServices.Application.DTOs.Locations;
using HomeServices.Domain.Enums;

namespace HomeServices.Application.DTOs.Orders;

/// <summary>
/// مجموعات تبويبات قوائم الطلبات.
/// </summary>
public enum OrderStatusGroup
{
    Active = 0,
    Completed = 1,
    Closed = 2,
    All = 3
}

public sealed record BookingServiceOption(int ServiceId, string Name, string CategoryName, decimal Price, PriceUnit PriceUnit);

/// <summary>
/// بيانات نموذج طلب خدمة من مقدم خدمة محدد.
/// </summary>
public sealed record BookingFormDataDto
{
    public required int ProviderId { get; init; }
    public required string ProviderName { get; init; }
    public string? ProviderHeadline { get; init; }
    public string? ProviderImagePath { get; init; }
    public decimal AverageRating { get; init; }
    public int ReviewsCount { get; init; }
    public bool IsAvailable { get; init; }
    public IReadOnlyList<BookingServiceOption> Services { get; init; } = [];
    public IReadOnlyList<CityDto> Areas { get; init; } = [];

    public bool CanBook => IsAvailable && Services.Count > 0 && Areas.Count > 0;
}

public sealed record CreateOrderInput(
    int ProviderId,
    int ServiceId,
    int AreaId,
    string AddressDetails,
    DateOnly ScheduledDate,
    TimeSlot TimeSlot,
    string Description);

public sealed record CreatedOrder(int Id, string OrderNumber);

public sealed record OrderSummaryDto(
    int Id,
    string OrderNumber,
    string ServiceName,
    string CategoryIcon,
    string CategoryAccentColor,
    string CounterpartName,
    string? CounterpartImagePath,
    string AreaName,
    string CityName,
    DateOnly ScheduledDate,
    TimeSlot TimeSlot,
    OrderStatus Status,
    decimal? AgreedPrice,
    DateTime CreatedAt);

public sealed record OrderCountsDto(int Active, int Completed, int Closed, int Pending)
{
    public int All => Active + Completed + Closed;
}

public sealed record OrderHistoryItemDto(OrderStatus? FromStatus, OrderStatus ToStatus, string? Note, string? ChangedByName, DateTime ChangedAt);

/// <summary>
/// تفاصيل الطلب كما يراها أحد طرفيه. بيانات التواصل والعنوان التفصيلي تُخفى عن مقدم الخدمة
/// حتى يقبل الطلب، وبيانات تواصل مقدم الخدمة تظهر للعميل بعد القبول.
/// </summary>
public sealed record OrderDetailsDto
{
    public required int Id { get; init; }
    public required string OrderNumber { get; init; }
    public required OrderStatus Status { get; init; }
    public required bool ViewerIsCustomer { get; init; }

    public required string ServiceName { get; init; }
    public required string CategoryName { get; init; }
    public required string CategoryIcon { get; init; }
    public required string CategoryAccentColor { get; init; }

    public required int ProviderProfileId { get; init; }
    public required string CounterpartName { get; init; }
    public string? CounterpartImagePath { get; init; }

    /// <summary>
    /// رقم هاتف الطرف الآخر؛ null حتى يُقبل الطلب.
    /// </summary>
    public string? CounterpartPhone { get; init; }

    public required string AreaName { get; init; }
    public required string CityName { get; init; }

    /// <summary>
    /// العنوان التفصيلي؛ null لمقدم الخدمة قبل قبول الطلب.
    /// </summary>
    public string? AddressDetails { get; init; }

    public required DateOnly ScheduledDate { get; init; }
    public required TimeSlot TimeSlot { get; init; }
    public required string Description { get; init; }
    public decimal? AgreedPrice { get; init; }
    public string? CancellationReason { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }

    public IReadOnlyList<OrderHistoryItemDto> History { get; init; } = [];
    public IReadOnlyList<OrderAction> AvailableActions { get; init; } = [];

    public bool ContactDetailsVisible => CounterpartPhone is not null;
}

public sealed record OrderActionInput(OrderAction Action, string? Note, decimal? Price);
