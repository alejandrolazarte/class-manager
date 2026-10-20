using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases;

namespace ClassManager.Infrastructure.Audit;

public sealed class QueryAuditBehavior<TQuery, TResponse>(UseCaseAuditRecorder recorder)
    : IUseCaseBehavior<TQuery, TResponse>
    where TQuery : IQuery
{
    private static readonly bool IsAudited = typeof(TQuery).IsDefined(typeof(AuditedQueryAttribute), inherit: false);

    public Task<Result<TResponse>> HandleAsync(TQuery query, UseCaseStep<TResponse> nextStep, CancellationToken cancellationToken) =>
        IsAudited ? recorder.RunAsync(query, nextStep) : nextStep();
}
