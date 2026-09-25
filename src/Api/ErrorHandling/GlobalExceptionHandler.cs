using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClassManager.Api.ErrorHandling;

internal sealed partial class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    private const string TenantNotResolvedTitle = "Tenant not resolved";
    private const string InvalidRequestBodyTitle = "Invalid request body";
    private const string InvalidRequestBodyDetail = "The request body could not be read. Send valid UTF-8 JSON with the expected field types.";
    private const string UnexpectedErrorTitle = "Unexpected error";
    private const string UnexpectedErrorDetail = "An unexpected error occurred.";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = CreateProblemDetails(exception);

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception,
        });
    }

    private ProblemDetails CreateProblemDetails(Exception exception)
    {
        var tenantNotResolvedException = FindInExceptionChain<TenantNotResolvedException>(exception);
        if (tenantNotResolvedException is not null)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = TenantNotResolvedTitle,
                Detail = tenantNotResolvedException.Message,
            };
        }

        var badHttpRequestException = FindInExceptionChain<BadHttpRequestException>(exception);
        if (badHttpRequestException is not null)
        {
            LogInvalidRequestBody(logger, badHttpRequestException.StatusCode, badHttpRequestException);
            return new ProblemDetails
            {
                Status = badHttpRequestException.StatusCode,
                Title = InvalidRequestBodyTitle,
                Detail = InvalidRequestBodyDetail,
            };
        }

        LogUnhandledException(logger, exception);
        return new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = UnexpectedErrorTitle,
            Detail = UnexpectedErrorDetail,
        };
    }

    private static TException? FindInExceptionChain<TException>(Exception exception)
        where TException : Exception
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is TException match)
            {
                return match;
            }
        }

        return null;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Rejected request with an unreadable body ({StatusCode}).")]
    private static partial void LogInvalidRequestBody(ILogger logger, int statusCode, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception while processing the request.")]
    private static partial void LogUnhandledException(ILogger logger, Exception exception);
}
