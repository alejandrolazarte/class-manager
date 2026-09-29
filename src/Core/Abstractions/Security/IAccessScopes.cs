namespace ClassManager.Core.Abstractions.Security;

public interface IAccessScopes
{
    Task<InstructorScope> ForInstructorsAsync(string everyInstructorPermission, CancellationToken cancellationToken);

    Task<ClientScope?> ForClientsAsync(CancellationToken cancellationToken);

    Task<ClientScope?> ForClientsAsync(string everyClientPermission, CancellationToken cancellationToken);
}
