namespace HomeServices.Application.DTOs.Stats;

public sealed record PlatformStatsDto(
    int ProvidersCount,
    int CompletedOrdersCount,
    int CategoriesCount,
    int CitiesCount);
