using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Members;

public sealed record RemoveMemberCommand(Guid MemberId);

public sealed record RemovedMemberResponse(Guid Id);

public sealed class RemoveMemberUseCase(
    IBusinessMemberRepository businessMemberRepository,
    ICurrentMember currentMember,
    IUnitOfWork unitOfWork)
    : IUseCase<RemoveMemberCommand, RemovedMemberResponse>
{
    public async Task<Result<RemovedMemberResponse>> ExecuteAsync(RemoveMemberCommand command, CancellationToken cancellationToken)
    {
        var member = await businessMemberRepository.GetForUpdateAsync(command.MemberId, cancellationToken);
        if (member is null)
        {
            return MemberRules.MemberNotFound();
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (member.UserId == access?.UserId)
        {
            return MemberRules.OwnMembership();
        }

        if (!await MemberRules.CanManageRoleAsync(currentMember, member.Role, cancellationToken))
        {
            return MemberRules.RoleNotAllowed();
        }

        businessMemberRepository.Remove(member);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RemovedMemberResponse(member.Id);
    }
}
