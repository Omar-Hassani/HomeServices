namespace HomeServices.Web.ViewModels.Shared;

public class AuthAsideViewModel
{
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required IReadOnlyList<(string Icon, string Text)> Points { get; init; }
}
