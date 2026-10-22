using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Services.AccessScopes;

internal sealed class CurrentMemberFake(BusinessRole role, long? instructorId) : ICurrentMember
{
    public Guid AccountId { get; } = Guid.CreateVersion7();

    public long UserId { get; } = TestIds.Next();

    public Task<MemberAccess?> GetAccessAsync(CancellationToken cancellationToken) =>
        Task.FromResult<MemberAccess?>(new MemberAccess(
            AccountId,
            UserId,
            TestIds.Next(),
            role,
            instructorId,
            IsBrandOwner: false,
            SystemRolePermissions.Of(role)));
}
