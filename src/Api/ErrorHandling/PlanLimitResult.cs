namespace ClassManager.Api.ErrorHandling;

internal sealed class PlanLimitResult(IResult problem) : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        PlanLimitResponses.Mark(httpContext);
        return problem.ExecuteAsync(httpContext);
    }
}
