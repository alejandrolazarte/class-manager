using ClassManager.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace ClassManager.Backoffice.Api.ErrorHandling;

internal static class BackofficeResultHttpExtensions
{
    public const string ErrorCodeExtension = "code";

    public static IResult ToOkResult<TValue>(this Result<TValue> result) =>
        result.Resolve(value => TypedResults.Ok(value), ToProblem);

    private static IResult ToProblem(ResultError error) =>
        TypedResults.Problem(new ProblemDetails
        {
            Status = error.Kind switch
            {
                ErrorKind.NotFound => StatusCodes.Status404NotFound,
                ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
                ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorKind.Validation => StatusCodes.Status400BadRequest,
                ErrorKind.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError,
            },
            Detail = error.Message,
            Extensions = { [ErrorCodeExtension] = error.Code },
        });
}
