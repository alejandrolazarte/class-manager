using ClassManager.Core.Common;
using ClassManager.Core.UseCases;

namespace ClassManager.Infrastructure.Audit;

public sealed class CommandAuditBehavior<TCommand, TResponse>(UseCaseAuditRecorder recorder)
    : IUseCaseBehavior<TCommand, TResponse>
    where TCommand : ICommand
{
    public Task<Result<TResponse>> HandleAsync(TCommand command, UseCaseStep<TResponse> nextStep, CancellationToken cancellationToken) =>
        recorder.RunAsync(command, nextStep);
}
