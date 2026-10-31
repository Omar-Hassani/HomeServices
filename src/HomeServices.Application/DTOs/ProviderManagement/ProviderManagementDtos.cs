using HomeServices.Domain.Enums;

namespace HomeServices.Application.DTOs.ProviderManagement;

public sealed record ProfileChecklistItem(string Key, string Label, bool IsDone, bool IsRequired);

public sealed record ProviderDashboardDto
{
    public required int ProfileId { get; init; }
    public required string FullName { get; init; }
    public string? ProfileImagePath { get; init; }
    public VerificationStatus VerificationStatus { get; init; }
    public string? VerificationNote { get; init; }
    public bool IsAvailable { get; init; }
    public decimal AverageRating { get; init; }
    public int ReviewsCount { get; init; }
    public int CompletedOrdersCount { get; init; }
    public int ServicesCount { get; init; }
    public int AreasCount { get; init; }
    public int PortfolioCount { get; init; }
    public int PendingOrdersCount { get; init; }
    public int ActiveOrdersCount { get; init; }
    public IReadOnlyList<ProfileChecklistItem> Checklist { get; init; } = [];

    public int CompletionPercent => Checklist.Count == 0 ? 0 : Checklist.Count(i => i.IsDone) * 100 / Checklist.Count;
    public bool IsComplete => Checklist.Where(i => i.IsRequired).All(i => i.IsDone);
}

public sealed record ProviderNavInfo(int ProfileId, VerificationStatus VerificationStatus, int CompletionPercent, int PendingOrdersCount);

public sealed record ProviderProfileEditDto(string Headline, string? Bio, int YearsOfExperience, int? CityId, bool IsAvailable);

public sealed record ProviderServiceInput(int ServiceId, decimal Price, PriceUnit PriceUnit, string? Notes);

public sealed record ManagedServiceDto(
    int Id,
    int ServiceId,
    string ServiceName,
    int CategoryId,
    string CategoryName,
    string CategoryIcon,
    string CategoryAccentColor,
    decimal Price,
    PriceUnit PriceUnit,
    string? Notes);

public sealed record ManagedPortfolioImageDto(int Id, string ImagePath, string? Caption);
