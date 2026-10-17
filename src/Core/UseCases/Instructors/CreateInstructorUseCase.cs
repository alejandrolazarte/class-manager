using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record CreateInstructorCommand(string? FullName, string? Email = null) : ICommand;

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
        var existingInstructor = await instructorRepository.FindByNameAsync(instructor.Value!.User.FullName, cancellationToken);
        if (existingInstructor is not null)
        {
            return InstructorFailures.NameTaken(existingInstructor.Id);
        }

        if (await PeopleEmails.TakenErrorAsync(userRepository, [(instructor.Value.User, nameof(CreateInstructorCommand.Email))], cancellationToken) is { } emailTaken)
        {
            return emailTaken;
        }

        instructorRepository.Add(instructor.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return InstructorResponse.From(instructor.Value);
    }
}
