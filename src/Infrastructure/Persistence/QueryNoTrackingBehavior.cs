using ClassManager.Core.Common;
using ClassManager.Core.UseCases;

namespace ClassManager.Infrastructure.Persistence;

public sealed class QueryNoTrackingBehavior<TQuery, TResponse>(AppDbContext context)
    : IUseCaseBehavior<TQuery, TResponse>
    where TQuery : IQuery
{
    public Task<Result<TResponse>> HandleAsync(TQuery command, UseCaseStep<TResponse> nextStep, CancellationToken cancellationToken)
    {
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        return nextStep();
    }
}
