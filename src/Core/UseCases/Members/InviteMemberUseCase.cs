using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Emails;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Members;

public sealed record InviteMemberCommand(
    [property: AuditPersonal] string? Email,
    BusinessRole? Role,
    long? InstructorId,
    long? CustomRoleId = null,
    long? SamePersonUserId = null) : ICommand;

public sealed class InviteMemberUseCase(
    IBusinessRepository businessRepository,
    IBusinessMemberRepository businessMemberRepository,
    IMemberInvitationRepository invitationRepository,
    IInstructorRepository instructorRepository,
    ICustomRoleRepository customRoleRepository,
    IIdentityService identityService,
    IUserRepository userRepository,
    IPeopleLock peopleLock,
    ICurrentMember currentMember,
    ISecretTokenGenerator secretTokenGenerator,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<InviteMemberCommand, InvitationResponse>
{
    private const string AlreadyMemberMessage = "This person is already part of the team.";
    private const string InstructorHasAccountMessage = "This instructor already uses the app with another account.";

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

        var existingAccountId = await identityService.FindUserIdByEmailAsync(invitation.Value!.Email, cancellationToken);
        if (existingAccountId is { } accountId && await businessMemberRepository.IsAccountMemberAsync(accountId, cancellationToken))
        {
            return Result.Conflict<InvitationResponse>(AlreadyMemberMessage, MemberErrorCodes.AlreadyMember);
        }

        foreach (var previousInvitation in await invitationRepository.ListPendingForUpdateByEmailAsync(invitation.Value.Email, now, cancellationToken))
        {
            previousInvitation.Revoke(now);
        }

        if (command.InstructorId is { } instructorId)
        {
            await peopleLock.LockAsync(cancellationToken);
            var instructor = await instructorRepository.GetForUpdateAsync(instructorId, cancellationToken);
            if (instructor!.User.AccountId is { } instructorAccountId && instructorAccountId != existingAccountId)
            {
                return Result.Conflict<InvitationResponse>(InstructorHasAccountMessage, MemberErrorCodes.InstructorTaken);
            }

            var samePerson = await SamePerson.FindAsync(
                userRepository,
                instructor.User,
                invitation.Value.Email,
                command.SamePersonUserId,
                roles => !roles.IsInstructor && !roles.IsTeamMember,
                nameof(InviteMemberCommand.Email),
                cancellationToken);
            if (samePerson.IsFailure)
            {
                return samePerson.Error!;
            }

            if (samePerson.Value is { } person)
            {
                var previousPerson = instructor.User;
                instructor.ChangePerson(person);
                person.CompleteWith(previousPerson);
                await SamePerson.RemoveIfUnusedAsync(
                    userRepository, previousPerson, roles => roles.IsClient || roles.IsStudent || roles.IsTeamMember, cancellationToken);
            }

            var emailChange = instructor.User.ChangeEmail(invitation.Value.Email);
            if (emailChange.IsFailure)
            {
                return emailChange.Error!;
            }

            if (await PeopleEmails.TakenErrorAsync(userRepository, [(instructor.User, nameof(InviteMemberCommand.Email))], cancellationToken) is { } emailTaken)
            {
                return emailTaken;
            }

            foreach (var previousInvitation in await invitationRepository.ListPendingForUpdateByInstructorAsync(instructorId, now, cancellationToken))
            {
                previousInvitation.Revoke(now);
            }
        }

        invitationRepository.Add(invitation.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var teamInvitation = EmailTexts.Spanish.TeamInvitation(business.BrandDisplayName, webAppLinks.AcceptInvitation(token.Value));
        await emailSender.SendAsync(new EmailMessage(invitation.Value.Email, teamInvitation.Subject, teamInvitation.Content, business.Id), cancellationToken);

        return InvitationResponse.From(invitation.Value);
    }
}
