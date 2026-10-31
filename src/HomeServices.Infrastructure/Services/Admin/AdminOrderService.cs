using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Admin;
using HomeServices.Application.DTOs.Orders;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Infrastructure.Services.Admin;

internal sealed class AdminOrderService(ApplicationDbContext db, OrderService orderService) : IAdminOrderService
{
    private const int PageSize = 20;

    public async Task<PagedResult<AdminOrderRowDto>> GetOrdersAsync(OrderStatus? status, string? query, int page, CancellationToken cancellationToken = default)
    {
        var orders = db.ServiceRequests.AsNoTracking();

        if (status is OrderStatus s)
        {
            orders = orders.Where(r => r.Status == s);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            orders = orders.Where(r =>
                r.OrderNumber.Contains(term) || r.Customer.FullName.Contains(term) || r.Provider.User.FullName.Contains(term));
        }

        var total = await orders.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var items = await orders
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(r => new AdminOrderRowDto(
                r.Id,
                r.OrderNumber,
                r.Service.Name,
                r.Service.Category.IconClass,
                r.Service.Category.AccentColor,
                r.Customer.FullName,
                r.Provider.User.FullName,
                r.Area.Name,
                r.Area.City.Name,
                r.ScheduledDate,
                r.Status,
                r.AgreedPrice,
                r.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminOrderRowDto>(items, total, page, PageSize);
    }

    public async Task<OrderCountsDto> GetCountsAsync(CancellationToken cancellationToken = default)
    {
        var counts = await db.ServiceRequests
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

    public async Task<AdminOrderDetailsDto?> GetDetailsAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderService.GetDetailsForAdminAsync(orderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        var parties = await db.ServiceRequests
            .AsNoTracking()
            .Where(r => r.Id == orderId)
            .Select(r => new
            {
                CustomerName = r.Customer.FullName,
                r.Customer.PhoneNumber,
                CustomerEmail = r.Customer.Email!,
                ProviderName = r.Provider.User.FullName,
                ProviderPhone = r.Provider.User.PhoneNumber
            })
            .FirstAsync(cancellationToken);

        return new AdminOrderDetailsDto(order, parties.CustomerName, parties.PhoneNumber, parties.CustomerEmail, parties.ProviderName, parties.ProviderPhone);
    }
}
