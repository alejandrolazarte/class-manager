using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases;

public interface IUseCase<in TCommand, TResponse>
{
    Task<Result<TResponse>> ExecuteAsync(TCommand command, CancellationToken cancellationToken);
}
