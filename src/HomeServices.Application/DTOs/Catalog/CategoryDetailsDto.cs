namespace HomeServices.Application.DTOs.Catalog;

public sealed record CategoryDetailsDto(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string IconClass,
    string AccentColor,
    IReadOnlyList<ServiceDto> Services);

public sealed record ServiceDto(int Id, int CategoryId, string Name, string Slug, int ProvidersCount);
