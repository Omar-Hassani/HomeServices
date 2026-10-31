using HomeServices.Application.DTOs.Locations;

namespace HomeServices.Application.Interfaces;

public interface ILocationService
{
    Task<IReadOnlyList<CityDto>> GetCitiesWithAreasAsync(CancellationToken cancellationToken = default);
}
