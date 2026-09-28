using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Services.AccessScopes;

internal sealed class CurrentMemberFake(BusinessRole role, Guid? instructorId) : ICurrentMember
{
    public Guid UserId { get; } = Guid.CreateVersion7();

    public Task<MemberAccess?> GetAccessAsync(CancellationToken cancellationToken) =>
        Task.FromResult<MemberAccess?>(new MemberAccess(
            UserId,
            Guid.CreateVersion7(),
            role,
            instructorId,
            IsBrandOwner: false,
            SystemRolePermissions.Of(role)));
}
