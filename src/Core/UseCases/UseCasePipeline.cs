using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases;

public sealed class UseCasePipeline<TCommand, TResponse>(
    IUseCase<TCommand, TResponse> useCase,
    IEnumerable<IUseCaseBehavior<TCommand, TResponse>> behaviors)
    : IUseCase<TCommand, TResponse>
{
    public Task<Result<TResponse>> ExecuteAsync(TCommand command, CancellationToken cancellationToken)
    {
        UseCaseStep<TResponse> nextStep = () => useCase.ExecuteAsync(command, cancellationToken);
        foreach (var behavior in behaviors.Reverse())
        {
            var innerStep = nextStep;
            nextStep = () => behavior.HandleAsync(command, innerStep, cancellationToken);
        }

        return nextStep();
    }
}
