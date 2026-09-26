using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record RestoreSessionScheduleCommand(Guid ClassGroupId, DateOnly Date);

public sealed class RestoreSessionScheduleUseCase(IClassSessionRepository sessionRepository, IUnitOfWork unitOfWork)
    : IUseCase<RestoreSessionScheduleCommand, SessionStatusResponse>
{
    public async Task<Result<SessionStatusResponse>> ExecuteAsync(RestoreSessionScheduleCommand command, CancellationToken cancellationToken)
    {
        var session = await sessionRepository.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        if (session is not null)
        {
            session.ClearReschedule();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new SessionStatusResponse(
            command.ClassGroupId, command.Date, session?.IsCancelled ?? false, session?.CancellationReason);
    }
}
