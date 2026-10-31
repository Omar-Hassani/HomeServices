using HomeServices.Domain.Enums;

namespace HomeServices.Application.DTOs.Providers;

/// <summary>
/// البيانات اللازمة لعرض بطاقة مقدم خدمة في القوائم ونتائج البحث.
/// </summary>
public sealed record ProviderCardDto(
    int Id,
    string FullName,
    string Headline,
    string? ProfileImagePath,
    string? CityName,
    string? PrimaryCategoryName,
    int YearsOfExperience,
    decimal AverageRating,
    int ReviewsCount,
    int CompletedOrdersCount,
    bool IsAvailable,
    decimal? StartingPrice,
    PriceUnit? StartingPriceUnit);
