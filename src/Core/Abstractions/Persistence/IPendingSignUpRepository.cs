using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IPendingSignUpRepository
{
    void Add(PendingSignUp pendingSignUp);

    Task<IReadOnlyList<PendingSignUp>> ListPendingForUpdateByEmailAsync(string email, DateTimeOffset now, CancellationToken cancellationToken);

    Task<PendingSignUp?> FindForUpdateByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
}
