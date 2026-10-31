using HomeServices.Application.DTOs.Locations;
using HomeServices.Application.Interfaces;
using HomeServices.Infrastructure.Caching;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HomeServices.Infrastructure.Services;

internal sealed class LocationService(ApplicationDbContext db, IMemoryCache cache) : ILocationService
{
    public async Task<IReadOnlyList<CityDto>> GetCitiesWithAreasAsync(CancellationToken cancellationToken = default) =>
        (await cache.GetOrCreateAsync(CatalogCache.Locations, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CatalogCache.Duration;
            IReadOnlyList<CityDto> cities = await db.Cities
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new CityDto(
                    c.Id,
                    c.Name,
                    c.Areas
                        .Where(a => a.IsActive)
                        .OrderBy(a => a.Id)
                        .Select(a => new AreaDto(a.Id, a.Name))
                        .ToList()))
                .ToListAsync(cancellationToken);
            return cities;
        }))!;
}
