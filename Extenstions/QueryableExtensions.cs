namespace IEEE.Extenstions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var currentPage = pageNumber < 1 ? 1 : pageNumber;
            var currentPageSize = pageSize < 1 ? 10 : pageSize;

            return query
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize);
        }
    }
}
