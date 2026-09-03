namespace Application.Common;

/// <summary>
/// Shared paging bounds so every list endpoint validates the same way.
/// </summary>
public static class PageRequest
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 25;
    public const int MaxPageSize = 100;

    public static int ToSkip(
        int page,
        int pageSize)
    {
        return (page - 1) * pageSize;
    }
}
