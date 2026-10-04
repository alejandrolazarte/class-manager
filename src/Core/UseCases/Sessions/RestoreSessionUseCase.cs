using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record RestoreSessionCommand(Guid ClassGroupId, DateOnly Date) : ICommand;

public sealed class RestoreSessionUseCase(IClassSessionRepository sessionRepository, IUnitOfWork unitOfWork)
    : IUseCase<RestoreSessionCommand, SessionStatusResponse>
{
    public async Task<Result<SessionStatusResponse>> ExecuteAsync(RestoreSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await sessionRepository.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        if (session is not null)
        {
            session.Restore();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new SessionStatusResponse(command.ClassGroupId, command.Date, IsCancelled: false, CancellationReason: null);
    }
}
