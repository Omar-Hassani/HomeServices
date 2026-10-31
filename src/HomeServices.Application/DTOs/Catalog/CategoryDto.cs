namespace HomeServices.Application.DTOs.Catalog;

public sealed record CategoryDto(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string IconClass,
    string AccentColor,
    int ProvidersCount);
