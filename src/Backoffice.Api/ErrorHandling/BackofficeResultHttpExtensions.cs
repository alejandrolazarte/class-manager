using System.Text.Json;
using ClassManager.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace ClassManager.Backoffice.Api.ErrorHandling;

internal static class BackofficeResultHttpExtensions
{
    public const string ErrorCodeExtension = "code";

    private const string GeneralFieldName = "";

    public static IResult ToOkResult<TValue>(this Result<TValue> result) =>
        result.Resolve(value => TypedResults.Ok(value), ToProblem);

    private static IResult ToProblem(ResultError error)
    {
        if (error.Kind == ErrorKind.Validation)
        {
            var fieldName = error.FieldName is null ? GeneralFieldName : JsonNamingPolicy.CamelCase.ConvertName(error.FieldName);
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]> { [fieldName] = [error.Message] },
                extensions: new Dictionary<string, object?> { [ErrorCodeExtension] = error.Code });
        }

        return TypedResults.Problem(new ProblemDetails
        {
            Status = error.Kind switch
            {
                ErrorKind.NotFound => StatusCodes.Status404NotFound,
                ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
                ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorKind.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError,
            },
            Detail = error.Message,
            Extensions = { [ErrorCodeExtension] = error.Code },
        });
    }
}
