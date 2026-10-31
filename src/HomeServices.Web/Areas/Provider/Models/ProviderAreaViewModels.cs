using System.ComponentModel.DataAnnotations;
using HomeServices.Application.DTOs.Catalog;
using HomeServices.Application.DTOs.Locations;
using HomeServices.Application.DTOs.ProviderManagement;
using HomeServices.Domain.Enums;

namespace HomeServices.Web.Areas.Provider.Models;

public class DashboardViewModel
{
    public required ProviderDashboardDto Dashboard { get; init; }

    /// <summary>
    /// رابط الصفحة التي يُستكمل فيها كل عنصر من قائمة اكتمال الملف.
    /// </summary>
    public required IReadOnlyDictionary<string, string> ChecklistLinks { get; init; }
}

public class ProfileFormViewModel
{
    [Required(ErrorMessage = "يرجى إدخال العنوان المهني.")]
    [StringLength(120, MinimumLength = 10, ErrorMessage = "يجب أن يكون العنوان المهني بين {2} و{1} حرفاً.")]
    [Display(Name = "العنوان المهني")]
    public string Headline { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "يجب ألا تتجاوز النبذة {1} حرف.")]
    [Display(Name = "نبذة تعريفية")]
    public string? Bio { get; set; }

    [Range(0, 60, ErrorMessage = "يجب أن تكون سنوات الخبرة بين {1} و{2}.")]
    [Display(Name = "سنوات الخبرة")]
    public int YearsOfExperience { get; set; }

    [Required(ErrorMessage = "يرجى اختيار المدينة.")]
    [Display(Name = "المدينة الرئيسية")]
    public int? CityId { get; set; }

    [Display(Name = "متاح لاستقبال طلبات جديدة")]
    public bool IsAvailable { get; set; } = true;

    public IReadOnlyList<CityDto> Cities { get; set; } = [];
}

public class ServiceFormModel
{
    [Required(ErrorMessage = "يرجى اختيار الخدمة.")]
    [Display(Name = "الخدمة")]
    public int? ServiceId { get; set; }

    [Required(ErrorMessage = "يرجى إدخال السعر.")]
    [Range(1, 1_000_000_000, ErrorMessage = "يرجى إدخال سعر صحيح.")]
    [Display(Name = "السعر (ل.س)")]
    public decimal? Price { get; set; }

    [Display(Name = "وحدة التسعير")]
    public PriceUnit PriceUnit { get; set; } = PriceUnit.PerVisit;

    [StringLength(300, ErrorMessage = "يجب ألا تتجاوز الملاحظات {1} حرف.")]
    [Display(Name = "ملاحظات (اختياري)")]
    public string? Notes { get; set; }
}

public class ServicesViewModel
{
    public required IReadOnlyList<ManagedServiceDto> Services { get; init; }
    public required IReadOnlyList<CategoryDetailsDto> Catalog { get; init; }
    public ServiceFormModel Form { get; init; } = new();
}

public class WorkAreasViewModel
{
    public required IReadOnlyList<CityDto> Cities { get; init; }
    public required IReadOnlySet<int> SelectedAreaIds { get; init; }
}

public class PortfolioViewModel
{
    public required IReadOnlyList<ManagedPortfolioImageDto> Images { get; init; }
    public required int MaxImages { get; init; }
    public bool CanUpload => Images.Count < MaxImages;
}
