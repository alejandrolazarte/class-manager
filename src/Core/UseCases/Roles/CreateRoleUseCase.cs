using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Roles;

namespace ClassManager.Core.UseCases.Roles;

public sealed record CreateRoleCommand([property: AuditPlain] string? Name, [property: AuditPlain] IReadOnlyList<string>? Permissions, [property: AuditPlain] string? CopiedFrom) : ICommand;

public sealed class CreateRoleUseCase(
    ICustomRoleRepository customRoleRepository,
    ICurrentMember currentMember,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<CreateRoleCommand, RoleResponse>
{
    public async Task<Result<RoleResponse>> ExecuteAsync(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = CustomRole.Create(command.Name, command.Permissions, command.CopiedFrom, timeProvider.GetUtcNow());
        if (role.IsFailure)
        {
            return role.Error!;
        }

        if (await RoleRules.CheckCanGrantAsync(currentMember, role.Value!, cancellationToken) is { } grantError)
        {
            return grantError;
        }

        customRoleRepository.Add(role.Value!);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return RoleFailures.NameTaken();
        }

        return RoleResponse.From(role.Value!, memberCount: 0);
    }
}
