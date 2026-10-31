using HomeServices.Application.DTOs.Locations;

namespace HomeServices.Web.ViewModels.Shared;

public class SearchBarViewModel
{
    public required IReadOnlyList<CityDto> Cities { get; init; }
    public IReadOnlyList<string> Suggestions { get; init; } = [];
    public string? Query { get; init; }
    public int? AreaId { get; init; }
}
