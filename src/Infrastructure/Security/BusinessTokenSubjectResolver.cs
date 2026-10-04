using ClassManager.Core.Abstractions.Security;
using ClassManager.Security.Tokens;

namespace ClassManager.Infrastructure.Security;

internal sealed class BusinessTokenSubjectResolver(IBranchDirectory branchDirectory, IStudentAppDirectory studentAppDirectory) : ITokenSubjectResolver
{
    public async Task<TokenSubject?> ResolveAsync(Guid userId, string email, Guid? tenantId, string? kind, CancellationToken cancellationToken) =>
        AccountKinds.IsStudent(kind)
            ? await ResolveStudentAsync(userId, email, tenantId, cancellationToken)
            : await ResolveTeamAsync(userId, email, tenantId, cancellationToken);

    private async Task<TokenSubject?> ResolveTeamAsync(Guid userId, string email, Guid? tenantId, CancellationToken cancellationToken)
    {
        var branch = tenantId is { } businessId
            ? await branchDirectory.FindAsync(userId, businessId, cancellationToken)
            : await branchDirectory.FindDefaultAsync(userId, cancellationToken);

        return branch is null ? null : new TokenSubject(userId, email, branch.BusinessId, branch.RoleName, AccountKinds.Team);
    }

    private async Task<TokenSubject?> ResolveStudentAsync(Guid userId, string email, Guid? tenantId, CancellationToken cancellationToken)
    {
        var student = tenantId is { } businessId
            ? await studentAppDirectory.FindAsync(userId, businessId, cancellationToken)
            : await studentAppDirectory.FindDefaultAsync(userId, cancellationToken);

        return student is null
            ? null
            : new TokenSubject(userId, email, student.BusinessId, AccountKinds.StudentRoleName, AccountKinds.Student);
    }
}
