using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Notifications.Email;

namespace ClassManager.Core.UseCases.Members;

public sealed record InviteMemberCommand(string? Email, BusinessRole? Role, Guid? InstructorId, Guid? CustomRoleId = null) : ICommand;

public sealed class InviteMemberUseCase(
    IBusinessRepository businessRepository,
    IBusinessMemberRepository businessMemberRepository,
    IMemberInvitationRepository invitationRepository,
    IInstructorRepository instructorRepository,
    ICustomRoleRepository customRoleRepository,
    IIdentityService identityService,
    ICurrentMember currentMember,
    ISecretTokenGenerator secretTokenGenerator,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<InviteMemberCommand, InvitationResponse>
{
    public const string EmailSubject = "Te invitaron a sumarte al equipo";

    private const string AlreadyMemberMessage = "This person is already part of the team.";

    public static EmailContent EmailContentFor(string businessName, string acceptInvitationLink) =>
        new(
            "Invitación al equipo",
            EmailSubject,
            $"Te invitaron a sumarte al equipo de {businessName}. Tocá el botón para aceptar.",
            "Si no esperabas esta invitación, podés ignorar este mail.")
        {
            Action = new EmailAction("Aceptar invitación", acceptInvitationLink, ShowsLinkFallback: true),
            Note = new EmailNote("El link sirve una sola vez y vence en 7 días."),
        };

    public async Task<Result<InvitationResponse>> ExecuteAsync(InviteMemberCommand command, CancellationToken cancellationToken)
    {
        var role = await MemberRules.ResolveRoleAsync(customRoleRepository, command.Role, command.CustomRoleId, cancellationToken);
        if (role.IsFailure)
        {
            return role.Error!;
        }

        if (await MemberRules.CheckCanGiveAsync(currentMember, role.Value!, cancellationToken) is { } roleError)
        {
            return roleError;
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (access is null || business is null)
        {
            return Result.Unauthorized<InvitationResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var now = timeProvider.GetUtcNow();
        var token = secretTokenGenerator.Create();
        var invitation = MemberInvitation.Create(command.Email, role.Value!, command.InstructorId, token.Hash, access.UserId, now);
        if (invitation.IsFailure)
        {
            return invitation.Error!;
        }

        var instructorError = await MemberRules.ValidateInstructorAsync(
            instructorRepository, businessMemberRepository, command.InstructorId, null, cancellationToken);
        if (instructorError is not null)
        {
            return instructorError;
        }

        var existingUserId = await identityService.FindUserIdByEmailAsync(invitation.Value!.Email, cancellationToken);
        if (existingUserId is { } userId && await businessMemberRepository.IsUserMemberAsync(userId, cancellationToken))
        {
            return Result.Conflict<InvitationResponse>(AlreadyMemberMessage, MemberErrorCodes.AlreadyMember);
        }

        foreach (var previousInvitation in await invitationRepository.ListPendingForUpdateByEmailAsync(invitation.Value.Email, now, cancellationToken))
        {
            previousInvitation.Revoke(now);
        }

        invitationRepository.Add(invitation.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var content = EmailContentFor(business.BrandDisplayName, webAppLinks.AcceptInvitation(token.Value));
        await emailSender.SendAsync(new EmailMessage(invitation.Value.Email, EmailSubject, content, business.Id), cancellationToken);

        return InvitationResponse.From(invitation.Value);
    }
}
