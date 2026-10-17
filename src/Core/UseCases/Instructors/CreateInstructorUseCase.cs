using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record CreateInstructorCommand(string? FullName, string? Email = null, Guid? SamePersonUserId = null) : ICommand;

public sealed class CreateInstructorUseCase(
    IInstructorRepository instructorRepository,
    IUserRepository userRepository,
    IPeopleLock peopleLock,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<CreateInstructorCommand, InstructorResponse>
{
    public async Task<Result<InstructorResponse>> ExecuteAsync(CreateInstructorCommand command, CancellationToken cancellationToken)
    {
        var instructor = Instructor.Create(command.FullName, timeProvider.GetUtcNow(), command.Email);
        if (instructor.IsFailure)
        {
            return instructor.Error!;
        }

        await peopleLock.LockAsync(cancellationToken);
        var samePerson = await SamePerson.FindAsync(
            userRepository,
            instructor.Value!.User,
            instructor.Value.User.Email,
            command.SamePersonUserId,
            roles => !roles.IsInstructor && !roles.IsTeamMember,
            nameof(CreateInstructorCommand.Email),
            cancellationToken);
        if (samePerson.IsFailure)
        {
            return samePerson.Error!;
        }

        var newInstructor = samePerson.Value is { } person ? Instructor.CreateFor(person) : instructor.Value;
        var existingInstructor = await instructorRepository.FindByNameAsync(newInstructor.User.FullName, cancellationToken);
        if (existingInstructor is not null)
        {
            return InstructorFailures.NameTaken(existingInstructor.Id);
        }

        instructorRepository.Add(newInstructor);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return InstructorResponse.From(newInstructor);
    }
}
