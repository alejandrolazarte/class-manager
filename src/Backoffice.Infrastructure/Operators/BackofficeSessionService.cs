using ClassManager.Backoffice.Core.Operators;
using ClassManager.Security.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Backoffice.Infrastructure.Operators;

internal sealed class BackofficeSessionService(SecurityDbContext securityContext, TimeProvider timeProvider) : IBackofficeSessionService
{
    public async Task RevokeAllAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        await securityContext.RefreshTokens
            .Where(token => token.UserId == accountId && token.Kind == BackofficeSession.Kind && token.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.RevokedAt, now), cancellationToken);
    }
}
