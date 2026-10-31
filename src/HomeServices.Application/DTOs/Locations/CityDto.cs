namespace HomeServices.Application.DTOs.Locations;

public sealed record CityDto(int Id, string Name, IReadOnlyList<AreaDto> Areas);

public sealed record AreaDto(int Id, string Name);
