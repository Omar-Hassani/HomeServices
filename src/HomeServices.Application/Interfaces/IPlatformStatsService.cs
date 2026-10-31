using HomeServices.Application.DTOs.Stats;

namespace HomeServices.Application.Interfaces;

public interface IPlatformStatsService
{
    Task<PlatformStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}
