using Microsoft.Extensions.Caching.Memory;

namespace HomeServices.Infrastructure.Caching;

/// <summary>
/// مفاتيح ومدة التخزين المؤقت لبيانات الكتالوج (الفئات والمناطق) التي تُقرأ في معظم الصفحات وتتغير نادراً.
/// كل تعديل إداري على الكتالوج أو على حالة توثيق مقدم خدمة يستدعي Invalidate.
/// </summary>
internal static class CatalogCache
{
    public const string ActiveCategories = "catalog:categories:active";
    public const string Catalog = "catalog:categories:full";
    public const string Locations = "catalog:locations";

    public static readonly TimeSpan Duration = TimeSpan.FromMinutes(10);

    public static void Invalidate(IMemoryCache cache)
    {
        cache.Remove(ActiveCategories);
        cache.Remove(Catalog);
        cache.Remove(Locations);
    }
}
