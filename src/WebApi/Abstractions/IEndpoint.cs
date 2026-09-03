namespace WebApi.Abstractions;

/// <summary>
/// One endpoint per implementing type. Feature modules map them with
/// <c>MapEndpoint&lt;T&gt;()</c>; the static abstract member makes a missing or misnamed
/// registration method a compile error rather than a route that silently disappears.
/// </summary>
internal interface IEndpoint
{
    static abstract void Map(
        IEndpointRouteBuilder app);
}
