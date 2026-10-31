namespace HomeServices.Web.ViewModels.Shared;

public class PaginationViewModel
{
    public required int Page { get; init; }
    public required int TotalPages { get; init; }
    public required Func<int, string> PageUrl { get; init; }

    /// <summary>
    /// أرقام الصفحات المعروضة؛ null تعني فاصلاً (…) بين المجموعات.
    /// </summary>
    public IEnumerable<int?> VisiblePages()
    {
        const int window = 1;
        int? previous = null;

        for (var page = 1; page <= TotalPages; page++)
        {
            var isEdge = page == 1 || page == TotalPages;
            var isNear = Math.Abs(page - Page) <= window;
            if (!isEdge && !isNear)
            {
                continue;
            }

            if (previous is not null && page - previous > 1)
            {
                yield return null;
            }

            yield return page;
            previous = page;
        }
    }
}
