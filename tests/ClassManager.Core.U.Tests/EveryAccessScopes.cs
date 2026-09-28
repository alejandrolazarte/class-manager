using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.U.Tests;

internal sealed class EveryAccessScopes : IAccessScopes
{
    public Task<InstructorScope> ForInstructorsAsync(string everyInstructorPermission, CancellationToken cancellationToken) =>
        Task.FromResult(InstructorScope.EveryInstructor);

    public Task<ClientScope?> ForClientsAsync(CancellationToken cancellationToken) => Task.FromResult<ClientScope?>(null);
}
