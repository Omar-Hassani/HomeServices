using HomeServices.Application.Constants;
using HomeServices.Application.DTOs.Admin;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Infrastructure.Services.Admin;

internal sealed class AdminDashboardService(ApplicationDbContext db) : IAdminDashboardService
{
    public async Task<AdminStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var weekAgo = DateTime.UtcNow.AddDays(-7);

        // DbContext لا يدعم الاستعلامات المتوازية، لذلك تُنفَّذ بالتتابع
        var usersCount = await db.Users.CountAsync(cancellationToken);
        var newUsers = await db.Users.CountAsync(u => u.CreatedAt >= weekAgo, cancellationToken);
        var customersCount = await CountInRoleAsync(Roles.Customer, cancellationToken);

        var providerStatuses = await db.ProviderProfiles
            .GroupBy(p => p.VerificationStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var ordersByStatus = await db.ServiceRequests
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var ordersLastWeek = await db.ServiceRequests.CountAsync(r => r.CreatedAt >= weekAgo, cancellationToken);

        var reviewStats = await db.Reviews
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Hidden = g.Count(r => r.IsHidden),
                Average = g.Where(r => !r.IsHidden).Average(r => (decimal?)r.Rating)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new AdminStatsDto
        {
            UsersCount = usersCount,
            CustomersCount = customersCount,
            NewUsersLast7Days = newUsers,
            ProvidersCount = providerStatuses.Sum(s => s.Count),
            ApprovedProvidersCount = providerStatuses.Where(s => s.Status == VerificationStatus.Approved).Sum(s => s.Count),
            PendingProvidersCount = providerStatuses.Where(s => s.Status == VerificationStatus.Pending).Sum(s => s.Count),
            OrdersCount = ordersByStatus.Sum(s => s.Count),
            OrdersLast7Days = ordersLastWeek,
            OrdersByStatus = Enum.GetValues<OrderStatus>()
                .ToDictionary(s => s, s => ordersByStatus.FirstOrDefault(o => o.Status == s)?.Count ?? 0),
            ReviewsCount = reviewStats?.Total ?? 0,
            HiddenReviewsCount = reviewStats?.Hidden ?? 0,
            PlatformAverageRating = Math.Round(reviewStats?.Average ?? 0, 1)
        };
    }

    private Task<int> CountInRoleAsync(string role, CancellationToken cancellationToken) =>
        (from userRole in db.UserRoles
         join r in db.Roles on userRole.RoleId equals r.Id
         where r.Name == role
         select userRole.UserId).CountAsync(cancellationToken);
}
