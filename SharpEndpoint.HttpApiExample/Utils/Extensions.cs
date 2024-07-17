namespace SharpEndpoint.HttpApiExample.Utils;

public static class Extensions
{
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> enumerable, int page, int limit)
    {
        if (page == 0) page = 1;
        if (limit == 0) limit = 1;

        return enumerable.Skip((page - 1) * limit).Take(limit);
    }

    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, int page, int limit)
    {
        if (page == 0) page = 1;
        if (limit == 0) limit = 1;

        return queryable.Skip((page - 1) * limit).Take(limit);
    }
}