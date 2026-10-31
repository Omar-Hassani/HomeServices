namespace HomeServices.Web.ViewModels.Shared;

public class ErrorViewModel
{
    public string? RequestId { get; init; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
