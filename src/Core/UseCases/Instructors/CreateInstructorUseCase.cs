using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record CreateInstructorCommand([property: AuditPersonal] string? FullName, [property: AuditPersonal] string? Email = null, Guid? SamePersonUserId = null) : ICommand;

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
            roles => !roles.IsInstructor,
            nameof(CreateInstructorCommand.Email),
            cancellationToken);
        if (samePerson.IsFailure)
        {
            return samePerson.Error!;
        }

        if (samePerson.Value is { } person)
        {
            person.CompleteWith(instructor.Value.User);
            instructor.Value.ChangePerson(person);
        }

        var existingInstructor = await instructorRepository.FindByNameAsync(instructor.Value.User.FullName, cancellationToken);
        if (existingInstructor is not null)
        {
            return InstructorFailures.NameTaken(existingInstructor.Id);
        }

        instructorRepository.Add(instructor.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return InstructorResponse.From(instructor.Value);
    }
}
