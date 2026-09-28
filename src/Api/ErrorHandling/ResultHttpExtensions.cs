using System.Text.Json;
using ClassManager.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace ClassManager.Api.ErrorHandling;

internal static class ResultHttpExtensions
{
    public const string ErrorCodeExtension = "code";
    private const string GeneralFieldName = "";

    public static IResult ToHttpResult<TValue>(this Result<TValue> result, Func<TValue, IResult> onSuccess) =>
        result.Resolve(onSuccess, ToProblem);

    public static IResult ToOkResult<TValue>(this Result<TValue> result) =>
        result.Resolve(value => TypedResults.Ok(value), ToProblem);

    private static IResult ToProblem(ResultError error)
    {
        if (error.Kind == ErrorKind.Validation)
        {
            var fieldName = error.FieldName is null
                ? GeneralFieldName
                : JsonNamingPolicy.CamelCase.ConvertName(error.FieldName);

            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]> { [fieldName] = [error.Message] },
                extensions: new Dictionary<string, object?> { [ErrorCodeExtension] = error.Code });
        }

        var extensions = new Dictionary<string, object?>(error.Details) { [ErrorCodeExtension] = error.Code };

        return TypedResults.Problem(new ProblemDetails
        {
            Status = ToStatusCode(error.Kind),
            Detail = error.Message,
            Extensions = extensions,
        });
    }

    private static int ToStatusCode(ErrorKind kind) => kind switch
    {
        ErrorKind.NotFound => StatusCodes.Status404NotFound,
        ErrorKind.Conflict => StatusCodes.Status409Conflict,
        ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
        ErrorKind.Locked => StatusCodes.Status423Locked,
        _ => StatusCodes.Status500InternalServerError,
    };
}
