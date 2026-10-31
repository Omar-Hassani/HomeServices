using HomeServices.Application.DTOs.Locations;
using HomeServices.Application.Interfaces;
using HomeServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Infrastructure.Services;

internal sealed class LocationService(ApplicationDbContext db) : ILocationService
{
    public async Task<IReadOnlyList<CityDto>> GetCitiesWithAreasAsync(CancellationToken cancellationToken = default)
    {
        return await db.Cities
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
    }
}
