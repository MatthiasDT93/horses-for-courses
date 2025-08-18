using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi;

public static class PagingExecution
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PageRequest request,
        CancellationToken ct = default) where T : class //where is nodig anders werkt AsNoTracking niet
    {
        var total = await query.CountAsync(ct);
        var pageItems = await query
            .ApplyPaging(request)
            .AsNoTracking() // meestal gewenst voor read‑only
            .ToListAsync(ct);

        return new PagedResult<T>(pageItems, total, request.Page, request.Size);
    }
}