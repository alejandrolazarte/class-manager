using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Members;

public sealed record ResendInvitationCommand(Guid InvitationId) : ICommand;

public sealed class ResendInvitationUseCase(
    IBusinessRepository businessRepository,
    IMemberInvitationRepository invitationRepository,
    ICurrentMember currentMember,
    ISecretTokenGenerator secretTokenGenerator,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<ResendInvitationCommand, InvitationResponse>
{
    private const string NotFoundMessage = "The invitation does not exist or was already used.";

    public async Task<Result<InvitationResponse>> ExecuteAsync(ResendInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await invitationRepository.GetForUpdateAsync(command.InvitationId, cancellationToken);
        if (invitation is null || !invitation.IsOpen)
        {
            return Result.NotFound<InvitationResponse>(NotFoundMessage, MemberErrorCodes.InvitationNotFound);
        }

        if (!await MemberRules.CanManageRoleAsync(currentMember, invitation.Role, cancellationToken))
        {
            return MemberRules.RoleNotAllowed();
        }

        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<InvitationResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var token = secretTokenGenerator.Create();
        invitation.Renew(token.Hash, timeProvider.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var content = InviteMemberUseCase.EmailContentFor(business.BrandDisplayName, webAppLinks.AcceptInvitation(token.Value));
        await emailSender.SendAsync(new EmailMessage(invitation.Email, InviteMemberUseCase.EmailSubjectFor(business.BrandDisplayName), content, business.Id), cancellationToken);

        return InvitationResponse.From(invitation);
    }
}
