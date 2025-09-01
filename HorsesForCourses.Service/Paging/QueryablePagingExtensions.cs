namespace HorsesForCourses.WebApi;

public static class QueryablePagingExtensions
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, PageRequest request)
    {
        // Defensive: als er geen OrderBy is, val terug op een stabiele volgorde (bijv. primary key)
        // Dit vereist dat T een Id heeft; anders injecteer je elders een OrderBy.
        if (!query.Expression.ToString().Contains("OrderBy"))
            throw new InvalidOperationException("Apply an OrderBy before paging to ensure stable results.");

        int skip = (request.Page - 1) * request.Size;
        return query.Skip(skip).Take(request.Size);
    }
}