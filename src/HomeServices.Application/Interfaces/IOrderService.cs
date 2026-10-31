using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Orders;

namespace HomeServices.Application.Interfaces;

public interface IOrderService
{
    public const int MaxBookingDaysAhead = 60;

    /// <summary>
    /// بيانات نموذج الطلب لمقدم خدمة ظاهر للعموم؛ null إن لم يكن الملف موثّقاً أو فعّالاً.
    /// </summary>
    Task<BookingFormDataDto?> GetBookingFormAsync(int providerId, CancellationToken cancellationToken = default);

    Task<Result<CreatedOrder>> CreateAsync(string customerId, CreateOrderInput input, CancellationToken cancellationToken = default);

    Task<PagedResult<OrderSummaryDto>> GetCustomerOrdersAsync(string customerId, OrderStatusGroup group, int page, CancellationToken cancellationToken = default);

    Task<PagedResult<OrderSummaryDto>> GetProviderOrdersAsync(string providerUserId, OrderStatusGroup group, int page, CancellationToken cancellationToken = default);

    Task<OrderCountsDto> GetCustomerCountsAsync(string customerId, CancellationToken cancellationToken = default);

    Task<OrderCountsDto> GetProviderCountsAsync(string providerUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// تفاصيل الطلب إن كان المستخدم أحد طرفيه (العميل أو مقدم الخدمة)، وإلا null.
    /// </summary>
    Task<OrderDetailsDto?> GetDetailsAsync(int orderId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ينفّذ إجراءً على الطلب بعد التحقق من أن المستخدم يملكه وأن الانتقال مسموح من الحالة الحالية.
    /// </summary>
    Task<Result> ExecuteActionAsync(int orderId, string userId, OrderActionInput input, CancellationToken cancellationToken = default);
}
