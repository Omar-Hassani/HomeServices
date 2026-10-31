using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Reviews;

namespace HomeServices.Application.Interfaces;

public interface IReviewService
{
    public const int MaxCommentLength = 1000;

    /// <summary>
    /// يضيف تقييم العميل لطلب مكتمل يملكه (مرة واحدة لكل طلب)، ويعيد حساب متوسط تقييم مقدم الخدمة.
    /// </summary>
    Task<Result> CreateAsync(string customerId, int orderId, ReviewInput input, CancellationToken cancellationToken = default);

    /// <summary>
    /// التقييمات الظاهرة لمقدم خدمة ظاهر للعموم؛ null إن لم يكن الملف ظاهراً.
    /// </summary>
    Task<ProviderReviewsPageDto?> GetPublicReviewsAsync(int providerId, int page, CancellationToken cancellationToken = default);

    /// <summary>
    /// تقييمات مقدم الخدمة الحالي في لوحته (بغض النظر عن حالة توثيق ملفه).
    /// </summary>
    Task<ProviderReviewsPageDto?> GetOwnReviewsAsync(string providerUserId, int page, CancellationToken cancellationToken = default);
}
