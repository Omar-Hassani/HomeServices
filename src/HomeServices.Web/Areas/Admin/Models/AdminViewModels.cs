using System.ComponentModel.DataAnnotations;
using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Admin;
using HomeServices.Application.DTOs.Orders;
using HomeServices.Domain.Enums;
using HomeServices.Web.ViewModels.Shared;

namespace HomeServices.Web.Areas.Admin.Models;

public class ProvidersPageViewModel
{
    public required PagedResult<AdminProviderRowDto> Providers { get; init; }
    public required PaginationViewModel Pagination { get; init; }
    public VerificationStatus? Status { get; init; }
    public string? Query { get; init; }
}

public class UsersPageViewModel
{
    public required PagedResult<AdminUserRowDto> Users { get; init; }
    public required PaginationViewModel Pagination { get; init; }
    public string? Query { get; init; }
    public string? Role { get; init; }
    public UserStatusFilter Status { get; init; }
    public required string CurrentUserId { get; init; }
}

public class OrdersPageViewModel
{
    public required PagedResult<AdminOrderRowDto> Orders { get; init; }
    public required PaginationViewModel Pagination { get; init; }
    public required OrderCountsDto Counts { get; init; }
    public OrderStatus? Status { get; init; }
    public string? Query { get; init; }
}

public class ReviewModerationPageViewModel
{
    public required PagedResult<AdminReviewRowDto> Reviews { get; init; }
    public required PaginationViewModel Pagination { get; init; }
    public ReviewVisibilityFilter Visibility { get; init; }
    public byte? Rating { get; init; }
}

public class CategoryFormModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "يرجى إدخال اسم الفئة.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "يجب أن يكون الاسم بين {2} و{1} حرفاً.")]
    [Display(Name = "اسم الفئة")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "يرجى إدخال المعرّف.")]
    [RegularExpression("^[a-z0-9]+(-[a-z0-9]+)*$", ErrorMessage = "أحرف إنجليزية صغيرة وأرقام وشرطات فقط، مثل: home-cleaning")]
    [StringLength(80)]
    [Display(Name = "المعرّف في الرابط (Slug)")]
    public string Slug { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "يجب ألا يتجاوز الوصف {1} حرف.")]
    [Display(Name = "الوصف")]
    public string? Description { get; set; }

    [StringLength(500, ErrorMessage = "يجب ألا تتجاوز الكلمات {1} حرف.")]
    [Display(Name = "كلمات البحث البديلة")]
    public string? SearchKeywords { get; set; }

    [Required(ErrorMessage = "يرجى إدخال الأيقونة.")]
    [RegularExpression("^fa-(solid|regular|brands) fa-[a-z0-9-]+$", ErrorMessage = "استخدم صيغة Font Awesome مثل: fa-solid fa-bolt")]
    [Display(Name = "الأيقونة")]
    public string IconClass { get; set; } = "fa-solid fa-house";

    [Required(ErrorMessage = "يرجى اختيار اللون.")]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "اللون بصيغة Hex مثل: #0E5E6F")]
    [Display(Name = "لون الفئة")]
    public string AccentColor { get; set; } = "#0E5E6F";

    [Range(0, 999, ErrorMessage = "ترتيب العرض بين {1} و{2}.")]
    [Display(Name = "ترتيب العرض")]
    public int DisplayOrder { get; set; }

    [Display(Name = "فعّالة وظاهرة في الموقع")]
    public bool IsActive { get; set; } = true;

    public CategoryInput ToInput() => new(Name, Slug, Description, SearchKeywords, IconClass, AccentColor, DisplayOrder, IsActive);
}

public class CategoryEditViewModel
{
    public required CategoryFormModel Form { get; init; }
    public IReadOnlyList<AdminServiceDto> Services { get; init; } = [];
    public bool IsNew => Form.Id is null;
}

public class ServiceFormModel
{
    public int? ServiceId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    // مربع الاختيار غير المحدد لا يُرسل شيئاً، لذا القيمة الافتراضية false حتى يعمل التعطيل
    public bool IsActive { get; set; }
}
