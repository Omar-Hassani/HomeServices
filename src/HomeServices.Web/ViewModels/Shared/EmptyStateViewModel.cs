namespace HomeServices.Web.ViewModels.Shared;

public class EmptyStateViewModel
{
    public required string Icon { get; init; }
    public required string Title { get; init; }
    public string? Message { get; init; }
    public string? ActionText { get; init; }
    public string? ActionUrl { get; init; }
}
