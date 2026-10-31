using HomeServices.Application.DTOs.Catalog;

namespace HomeServices.Application.Interfaces;

public interface ICategoryService
{
    /// <summary>
    /// الفئات الرئيسية الفعّالة مرتبة حسب ترتيب العرض، مع عدد مقدمي الخدمات الموثّقين في كل فئة.
    /// </summary>
    Task<IReadOnlyList<CategoryDto>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// كل الفئات الفعّالة مع خدماتها الفعّالة (لقوائم التصفية).
    /// </summary>
    Task<IReadOnlyList<CategoryDetailsDto>> GetCatalogAsync(CancellationToken cancellationToken = default);

    Task<CategoryDetailsDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
