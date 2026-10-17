using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record SeparateInstructorCommand(Guid InstructorId, string? FullName, string? Email) : ICommand;

public sealed class SeparateInstructorUseCase(
    IInstructorRepository instructorRepository,
    IBusinessMemberRepository businessMemberRepository,
    IMemberInvitationRepository invitationRepository,
    IUserRepository userRepository,
    IPeopleLock peopleLock,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<SeparateInstructorCommand, InstructorResponse>
{
    private const string OnTheTeamMessage = "The instructor is linked to a team member. Remove them from the team before separating.";

    public async Task<Result<InstructorResponse>> ExecuteAsync(SeparateInstructorCommand command, CancellationToken cancellationToken)
    {
        await peopleLock.LockAsync(cancellationToken);
        var instructor = await instructorRepository.GetForUpdateAsync(command.InstructorId, cancellationToken);
        if (instructor is null)
        {
            return InstructorFailures.NotFound();
        }

        var roles = await userRepository.GetRolesAsync(instructor.UserId, cancellationToken);
        if (!roles.IsClient && !roles.IsStudent)
        {
            return SeparatedPerson.NothingToSeparate();
        }

        if (await businessMemberRepository.IsInstructorLinkedAsync(instructor.Id, null, cancellationToken))
        {
            return Result.Conflict<InstructorResponse>(OnTheTeamMessage, InstructorErrorCodes.OnTheTeam);
        }

        var now = timeProvider.GetUtcNow();
        var person = await SeparatedPerson.CreateAsync(userRepository, command.FullName, command.Email, now, cancellationToken);
        if (person.IsFailure)
        {
            return person.Error!;
        }

        var existingInstructor = await instructorRepository.FindByNameAsync(person.Value!.FullName, cancellationToken);
        if (existingInstructor is not null && existingInstructor.Id != instructor.Id)
        {
            return InstructorFailures.NameTaken(existingInstructor.Id);
        }

        foreach (var invitation in await invitationRepository.ListPendingForUpdateByInstructorAsync(instructor.Id, now, cancellationToken))
        {
            invitation.Revoke(now);
        }

        instructor.ChangePerson(person.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return InstructorResponse.From(instructor);
    }
}
