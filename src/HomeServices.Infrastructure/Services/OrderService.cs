using System.Security.Cryptography;
using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Locations;
using HomeServices.Application.DTOs.Orders;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Entities;
using HomeServices.Domain.Enums;
using HomeServices.Domain.Workflow;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServices.Infrastructure.Services;

internal sealed class OrderService(ApplicationDbContext db, ILogger<OrderService> logger) : IOrderService
{
    private const int PageSize = 10;
    private const int MinReasonLength = 5;
    private const decimal MaxPrice = 1_000_000_000;

    // حروف رقم الطلب بدون الأحرف المتشابهة (0/O، 1/I)
    private const string OrderNumberAlphabet = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";

    private static readonly OrderStatus[] ContactVisibleStatuses = [OrderStatus.Accepted, OrderStatus.InProgress, OrderStatus.Completed];

    public async Task<BookingFormDataDto?> GetBookingFormAsync(int providerId, CancellationToken cancellationToken = default)
    {
        var provider = await db.ProviderProfiles
            .AsNoTracking()
            .WherePubliclyVisible()
            .Where(p => p.Id == providerId)
            .Select(p => new
            {
                p.Id,
                p.User.FullName,
                p.Headline,
                p.User.ProfileImagePath,
                p.AverageRating,
                p.ReviewsCount,
                p.IsAvailable
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (provider is null)
        {
            return null;
        }

        var services = await db.ProviderServices
            .AsNoTracking()
            .Where(s => s.ProviderProfileId == providerId && s.Service.IsActive && s.Service.Category.IsActive)
            .OrderBy(s => s.Service.Category.DisplayOrder)
            .ThenBy(s => s.Service.DisplayOrder)
            .Select(s => new BookingServiceOption(s.ServiceId, s.Service.Name, s.Service.Category.Name, s.Price, s.PriceUnit))
            .ToListAsync(cancellationToken);

        var areas = await db.ProviderServiceAreas
            .AsNoTracking()
            .Where(a => a.ProviderProfileId == providerId && a.Area.IsActive && a.Area.City.IsActive)
            .OrderBy(a => a.Area.City.DisplayOrder)
            .ThenBy(a => a.AreaId)
            .Select(a => new { a.Area.CityId, CityName = a.Area.City.Name, a.AreaId, AreaName = a.Area.Name })
            .ToListAsync(cancellationToken);

        return new BookingFormDataDto
        {
            ProviderId = provider.Id,
            ProviderName = provider.FullName,
            ProviderHeadline = provider.Headline,
            ProviderImagePath = provider.ProfileImagePath,
            AverageRating = provider.AverageRating,
            ReviewsCount = provider.ReviewsCount,
            IsAvailable = provider.IsAvailable,
            Services = services,
            Areas = areas
                .GroupBy(a => (a.CityId, a.CityName))
                .Select(g => new CityDto(g.Key.CityId, g.Key.CityName, g.Select(a => new AreaDto(a.AreaId, a.AreaName)).ToList()))
                .ToList()
        };
    }

    public async Task<Result<CreatedOrder>> CreateAsync(string customerId, CreateOrderInput input, CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(input.TimeSlot))
        {
            return Result<CreatedOrder>.Failure("الفترة الزمنية غير صالحة.");
        }

        var today = SyriaTime.Today;
        if (input.ScheduledDate < today || input.ScheduledDate > today.AddDays(IOrderService.MaxBookingDaysAhead))
        {
            return Result<CreatedOrder>.Failure($"يجب أن يكون الموعد بين اليوم و{IOrderService.MaxBookingDaysAhead} يوماً من الآن.");
        }

        var provider = await db.ProviderProfiles
            .AsNoTracking()
            .WherePubliclyVisible()
            .Where(p => p.Id == input.ProviderId)
            .Select(p => new { p.Id, p.UserId, p.IsAvailable })
            .FirstOrDefaultAsync(cancellationToken);

        if (provider is null)
        {
            return Result<CreatedOrder>.Failure("مقدم الخدمة غير متاح.");
        }

        if (provider.UserId == customerId)
        {
            return Result<CreatedOrder>.Failure("لا يمكنك طلب خدمة من نفسك.");
        }

        if (!provider.IsAvailable)
        {
            return Result<CreatedOrder>.Failure("مقدم الخدمة لا يستقبل طلبات جديدة حالياً.");
        }

        var offered = await db.ProviderServices
            .AsNoTracking()
            .Where(s => s.ProviderProfileId == provider.Id && s.ServiceId == input.ServiceId && s.Service.IsActive)
            .Select(s => new { s.Price })
            .FirstOrDefaultAsync(cancellationToken);

        if (offered is null)
        {
            return Result<CreatedOrder>.Failure("الخدمة المختارة لا يقدمها مقدم الخدمة هذا.");
        }

        var coversArea = await db.ProviderServiceAreas
            .AnyAsync(a => a.ProviderProfileId == provider.Id && a.AreaId == input.AreaId && a.Area.IsActive, cancellationToken);

        if (!coversArea)
        {
            return Result<CreatedOrder>.Failure("مقدم الخدمة لا يعمل في المنطقة المختارة.");
        }

        var hasPendingDuplicate = await db.ServiceRequests.AnyAsync(r =>
            r.CustomerId == customerId &&
            r.ProviderProfileId == provider.Id &&
            r.ServiceId == input.ServiceId &&
            r.Status == OrderStatus.Pending, cancellationToken);

        if (hasPendingDuplicate)
        {
            return Result<CreatedOrder>.Failure("لديك طلب سابق لنفس الخدمة لدى مقدم الخدمة هذا ما زال بانتظار الرد.");
        }

        var request = new ServiceRequest
        {
            OrderNumber = await GenerateOrderNumberAsync(cancellationToken),
            CustomerId = customerId,
            ProviderProfileId = provider.Id,
            ServiceId = input.ServiceId,
            AreaId = input.AreaId,
            AddressDetails = input.AddressDetails.Trim(),
            ScheduledDate = input.ScheduledDate,
            TimeSlot = input.TimeSlot,
            Description = input.Description.Trim(),
            AgreedPrice = offered.Price,
            Status = OrderStatus.Pending
        };

        request.StatusHistory.Add(new OrderStatusHistory
        {
            FromStatus = null,
            ToStatus = OrderStatus.Pending,
            ChangedByUserId = customerId,
            ChangedAt = DateTime.UtcNow
        });

        db.ServiceRequests.Add(request);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderNumber} created by {CustomerId} for provider {ProviderId}", request.OrderNumber, customerId, provider.Id);
        return Result<CreatedOrder>.Success(new CreatedOrder(request.Id, request.OrderNumber));
    }

    public Task<PagedResult<OrderSummaryDto>> GetCustomerOrdersAsync(string customerId, OrderStatusGroup group, int page, CancellationToken cancellationToken = default) =>
        ListAsync(db.ServiceRequests.Where(r => r.CustomerId == customerId), group, page, viewerIsCustomer: true, cancellationToken);

    public Task<PagedResult<OrderSummaryDto>> GetProviderOrdersAsync(string providerUserId, OrderStatusGroup group, int page, CancellationToken cancellationToken = default) =>
        ListAsync(db.ServiceRequests.Where(r => r.Provider.UserId == providerUserId), group, page, viewerIsCustomer: false, cancellationToken);

    public Task<OrderCountsDto> GetCustomerCountsAsync(string customerId, CancellationToken cancellationToken = default) =>
        CountAsync(db.ServiceRequests.Where(r => r.CustomerId == customerId), cancellationToken);

    public Task<OrderCountsDto> GetProviderCountsAsync(string providerUserId, CancellationToken cancellationToken = default) =>
        CountAsync(db.ServiceRequests.Where(r => r.Provider.UserId == providerUserId), cancellationToken);

    public async Task<OrderDetailsDto?> GetDetailsAsync(int orderId, string userId, CancellationToken cancellationToken = default)
    {
        var data = await db.ServiceRequests
            .AsNoTracking()
            .Where(r => r.Id == orderId && (r.CustomerId == userId || r.Provider.UserId == userId))
            .Select(r => new
            {
                r.Id,
                r.OrderNumber,
                r.Status,
                r.CustomerId,
                CustomerName = r.Customer.FullName,
                CustomerImage = r.Customer.ProfileImagePath,
                CustomerPhone = r.Customer.PhoneNumber,
                r.ProviderProfileId,
                ProviderName = r.Provider.User.FullName,
                ProviderImage = r.Provider.User.ProfileImagePath,
                ProviderPhone = r.Provider.User.PhoneNumber,
                ServiceName = r.Service.Name,
                CategoryName = r.Service.Category.Name,
                CategoryIcon = r.Service.Category.IconClass,
                CategoryAccentColor = r.Service.Category.AccentColor,
                AreaName = r.Area.Name,
                CityName = r.Area.City.Name,
                r.AddressDetails,
                r.ScheduledDate,
                r.TimeSlot,
                r.Description,
                r.AgreedPrice,
                r.CancellationReason,
                r.CreatedAt,
                r.CompletedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (data is null)
        {
            return null;
        }

        var history = await db.OrderStatusHistory
            .AsNoTracking()
            .Where(h => h.ServiceRequestId == orderId)
            .OrderBy(h => h.ChangedAt)
            .Select(h => new OrderHistoryItemDto(h.FromStatus, h.ToStatus, h.Note, h.ChangedByUser != null ? h.ChangedByUser.FullName : null, h.ChangedAt))
            .ToListAsync(cancellationToken);

        var review = await db.Reviews
            .AsNoTracking()
            .Where(r => r.ServiceRequestId == orderId)
            .Select(r => new OrderReviewDto(r.Rating, r.Comment, r.CreatedAt, r.IsHidden))
            .FirstOrDefaultAsync(cancellationToken);

        var isCustomer = data.CustomerId == userId;
        var contactVisible = ContactVisibleStatuses.Contains(data.Status);

        return new OrderDetailsDto
        {
            Id = data.Id,
            OrderNumber = data.OrderNumber,
            Status = data.Status,
            ViewerIsCustomer = isCustomer,
            ServiceName = data.ServiceName,
            CategoryName = data.CategoryName,
            CategoryIcon = data.CategoryIcon,
            CategoryAccentColor = data.CategoryAccentColor,
            ProviderProfileId = data.ProviderProfileId,
            CounterpartName = isCustomer ? data.ProviderName : data.CustomerName,
            CounterpartImagePath = isCustomer ? data.ProviderImage : data.CustomerImage,
            CounterpartPhone = contactVisible ? (isCustomer ? data.ProviderPhone : data.CustomerPhone) : null,
            AreaName = data.AreaName,
            CityName = data.CityName,
            AddressDetails = isCustomer || contactVisible ? data.AddressDetails : null,
            ScheduledDate = data.ScheduledDate,
            TimeSlot = data.TimeSlot,
            Description = data.Description,
            AgreedPrice = data.AgreedPrice,
            CancellationReason = data.CancellationReason,
            CreatedAt = data.CreatedAt,
            CompletedAt = data.CompletedAt,
            History = history,
            AvailableActions = ServiceRequestWorkflow.AvailableActions(data.Status, isCustomer),
            Review = review
        };
    }

    public async Task<Result> ExecuteActionAsync(int orderId, string userId, OrderActionInput input, CancellationToken cancellationToken = default)
    {
        var request = await db.ServiceRequests
            .Include(r => r.Provider)
            .FirstOrDefaultAsync(r => r.Id == orderId && (r.CustomerId == userId || r.Provider.UserId == userId), cancellationToken);

        if (request is null)
        {
            return Result.Failure("الطلب غير موجود.");
        }

        var isCustomer = request.CustomerId == userId;
        if (!ServiceRequestWorkflow.CanPerform(request.Status, input.Action, isCustomer))
        {
            return Result.Failure("لا يمكن تنفيذ هذا الإجراء على الطلب في حالته الحالية.");
        }

        var note = string.IsNullOrWhiteSpace(input.Note) ? null : input.Note.Trim();
        if (ServiceRequestWorkflow.RequiresReason(input.Action) && (note is null || note.Length < MinReasonLength))
        {
            return Result.Failure($"يرجى ذكر السبب ({MinReasonLength} أحرف على الأقل).");
        }

        if (input.Price is decimal price && (price <= 0 || price > MaxPrice))
        {
            return Result.Failure("السعر المدخل غير صالح.");
        }

        var previousStatus = request.Status;
        var newStatus = ServiceRequestWorkflow.TargetStatus(input.Action);
        request.Status = newStatus;

        if (input.Action is OrderAction.Accept or OrderAction.Complete && input.Price is decimal agreedPrice)
        {
            request.AgreedPrice = agreedPrice;
        }

        if (newStatus is OrderStatus.Rejected or OrderStatus.Cancelled)
        {
            request.CancellationReason = note;
        }

        if (newStatus == OrderStatus.Completed)
        {
            request.CompletedAt = DateTime.UtcNow;
            request.Provider.CompletedOrdersCount++;
        }

        db.OrderStatusHistory.Add(new OrderStatusHistory
        {
            ServiceRequestId = request.Id,
            FromStatus = previousStatus,
            ToStatus = newStatus,
            ChangedByUserId = userId,
            Note = note,
            ChangedAt = DateTime.UtcNow
        });

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Status رمز تزامن: إن غيّر الطرف الآخر الحالة في نفس اللحظة لا نطبّق انتقالاً قديماً
            return Result.Failure("تم تحديث الطلب للتو من الطرف الآخر. حدّث الصفحة وحاول مجدداً.");
        }

        logger.LogInformation("Order {OrderNumber}: {From} -> {To} by {UserId}", request.OrderNumber, previousStatus, newStatus, userId);
        return Result.Success();
    }

    private async Task<PagedResult<OrderSummaryDto>> ListAsync(
        IQueryable<ServiceRequest> query, OrderStatusGroup group, int page, bool viewerIsCustomer, CancellationToken cancellationToken)
    {
        query = group switch
        {
            OrderStatusGroup.Active => query.Where(r => r.Status == OrderStatus.Pending || r.Status == OrderStatus.Accepted || r.Status == OrderStatus.InProgress),
            OrderStatusGroup.Completed => query.Where(r => r.Status == OrderStatus.Completed),
            OrderStatusGroup.Closed => query.Where(r => r.Status == OrderStatus.Rejected || r.Status == OrderStatus.Cancelled),
            _ => query
        };

        var total = await query.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        // الطلبات النشطة تُرتب حسب الموعد الأقرب، والبقية حسب الأحدث
        var ordered = group == OrderStatusGroup.Active
            ? query.OrderBy(r => r.ScheduledDate).ThenBy(r => r.TimeSlot).ThenBy(r => r.Id)
            : query.OrderByDescending(r => r.CreatedAt).ThenByDescending(r => r.Id);

        var items = await ordered
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .AsNoTracking()
            .Select(r => new OrderSummaryDto(
                r.Id,
                r.OrderNumber,
                r.Service.Name,
                r.Service.Category.IconClass,
                r.Service.Category.AccentColor,
                viewerIsCustomer ? r.Provider.User.FullName : r.Customer.FullName,
                viewerIsCustomer ? r.Provider.User.ProfileImagePath : r.Customer.ProfileImagePath,
                r.Area.Name,
                r.Area.City.Name,
                r.ScheduledDate,
                r.TimeSlot,
                r.Status,
                r.AgreedPrice,
                r.CreatedAt,
                r.Review != null))
            .ToListAsync(cancellationToken);

        return new PagedResult<OrderSummaryDto>(items, total, page, PageSize);
    }

    private static async Task<OrderCountsDto> CountAsync(IQueryable<ServiceRequest> query, CancellationToken cancellationToken)
    {
        var counts = await query
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int Sum(params OrderStatus[] statuses) => counts.Where(c => statuses.Contains(c.Status)).Sum(c => c.Count);

        return new OrderCountsDto(
            Active: Sum(OrderStatus.Pending, OrderStatus.Accepted, OrderStatus.InProgress),
            Completed: Sum(OrderStatus.Completed),
            Closed: Sum(OrderStatus.Rejected, OrderStatus.Cancelled),
            Pending: Sum(OrderStatus.Pending));
    }

    /// <summary>
    /// رقم طلب قصير وسهل القراءة بالشكل HS-261031-7K3F، مع التحقق من عدم التكرار.
    /// </summary>
    private async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken)
    {
        var datePart = SyriaTime.Today.ToString("yyMMdd");
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var suffix = new string(Enumerable.Range(0, 4)
                .Select(_ => OrderNumberAlphabet[RandomNumberGenerator.GetInt32(OrderNumberAlphabet.Length)])
                .ToArray());
            var candidate = $"HS-{datePart}-{suffix}";

            if (!await db.ServiceRequests.AnyAsync(r => r.OrderNumber == candidate, cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Could not generate a unique order number.");
    }
}
