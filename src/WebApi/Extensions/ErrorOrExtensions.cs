using ErrorOr;

namespace WebApi.Extensions;

/// <summary>
/// Translates handler results into HTTP responses in one place, so endpoints do not each
/// invent their own status code mapping.
/// </summary>
internal static class ErrorOrExtensions
{
    public static IResult Match<TValue>(
        this ErrorOr<TValue> result,
        Func<TValue, IResult> onSuccess)
    {
        return result.IsError
            ? ToProblem(result.Errors)
            : onSuccess(result.Value);
    }

    public static IResult ToOk<TValue>(
        this ErrorOr<TValue> result)
    {
        return result.Match(Results.Ok);
    }

    public static IResult ToNoContent<TValue>(
        this ErrorOr<TValue> result)
    {
        return result.Match(_ => Results.NoContent());
    }

    public static IResult ToCreated<TValue>(
        this ErrorOr<TValue> result,
        Func<TValue, string> location)
    {
        return result.Match(value => Results.Created(
            new Uri(location(value), UriKind.Relative),
            value));
    }

    private static IResult ToProblem(
        List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        // Validation failures carry one entry per field, so they are returned together
        // rather than collapsed into whichever one happened to be first.
        if (errors.TrueForAll(e => e.Type == ErrorType.Validation))
        {
            return Results.ValidationProblem(
                errors
                    .GroupBy(e => e.Code, StringComparer.Ordinal)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(e => e.Description).ToArray(),
                        StringComparer.Ordinal));
        }

        var error = errors[0];

        return error.Type switch
        {
            ErrorType.NotFound => Results.Problem(
                detail: error.Description,
                statusCode: StatusCodes.Status404NotFound,
                title: "Not found"),

            ErrorType.Conflict => Results.Problem(
                detail: error.Description,
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict"),

            ErrorType.Unauthorized => Results.Problem(
                detail: error.Description,
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized"),

            ErrorType.Forbidden => Results.Problem(
                detail: error.Description,
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden"),

            ErrorType.Validation => Results.Problem(
                detail: error.Description,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad request"),

            // Unexpected and Failure both mean the caller cannot act on the detail, so it
            // is logged by the handler and not echoed back.
            _ => Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred")
        };
    }
}
