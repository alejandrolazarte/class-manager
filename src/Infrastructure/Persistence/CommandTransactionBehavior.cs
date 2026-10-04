using ClassManager.Core.Common;
using ClassManager.Core.UseCases;

namespace ClassManager.Infrastructure.Persistence;

public sealed class CommandTransactionBehavior<TCommand, TResponse>(AppDbContext context, IUnitOfWork unitOfWork)
    : IUseCaseBehavior<TCommand, TResponse>
    where TCommand : ICommand
{
    public async Task<Result<TResponse>> HandleAsync(TCommand command, UseCaseStep<TResponse> nextStep, CancellationToken cancellationToken)
    {
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        var result = await nextStep();
        await transaction.CommitAsync(cancellationToken);

        return result;
    }
}
