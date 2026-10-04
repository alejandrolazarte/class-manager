using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record RemoveSubstituteCommand(Guid ClassGroupId, DateOnly Date) : ICommand;

public sealed class RemoveSubstituteUseCase(IClassSessionRepository sessionRepository, IUnitOfWork unitOfWork)
    : IUseCase<RemoveSubstituteCommand, SessionStatusResponse>
{
    public async Task<Result<SessionStatusResponse>> ExecuteAsync(RemoveSubstituteCommand command, CancellationToken cancellationToken)
    {
        var session = await sessionRepository.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        if (session is not null)
        {
            session.ClearSubstitute();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new SessionStatusResponse(
            command.ClassGroupId, command.Date, session?.IsCancelled ?? false, session?.CancellationReason);
    }
}
