namespace TaskManagement.Api.Models.Common;

public class CursorPaginatedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
    public Guid? NextCursor { get; set; }

    public CursorPaginatedResult() {}

    public CursorPaginatedResult(List<T> items, int pageSize, bool hasNextPage, Guid? nextCursor)
    {
        Items = items;
        PageSize = pageSize;
        HasNextPage = hasNextPage;
        NextCursor = nextCursor;
    }
}
