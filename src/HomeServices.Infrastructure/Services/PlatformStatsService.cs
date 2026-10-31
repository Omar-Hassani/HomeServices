using HomeServices.Application.DTOs.Stats;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Infrastructure.Services;

internal sealed class PlatformStatsService(ApplicationDbContext db) : IPlatformStatsService
{
    public async Task<PlatformStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        // DbContext لا يدعم الاستعلامات المتوازية، لذلك تُنفَّذ بالتتابع
        var providers = await db.ProviderProfiles.WherePubliclyVisible().CountAsync(cancellationToken);
        var completedOrders = await db.ServiceRequests.CountAsync(r => r.Status == OrderStatus.Completed, cancellationToken);
        var categories = await db.ServiceCategories.CountAsync(c => c.IsActive && c.ParentCategoryId == null, cancellationToken);
        var cities = await db.Cities.CountAsync(c => c.IsActive, cancellationToken);

        return new PlatformStatsDto(providers, completedOrders, categories, cities);
    }
}
