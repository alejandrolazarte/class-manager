using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases;

public delegate Task<Result<TResponse>> UseCaseStep<TResponse>();

public interface IUseCaseBehavior<in TCommand, TResponse>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, UseCaseStep<TResponse> nextStep, CancellationToken cancellationToken);
}
