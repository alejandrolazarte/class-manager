using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record CreateInstructorCommand(string? FullName, string? Email = null) : ICommand;

public sealed class CreateInstructorUseCase(IInstructorRepository instructorRepository, IUnitOfWork unitOfWork)
    : IUseCase<CreateInstructorCommand, InstructorResponse>
{
    public async Task<Result<InstructorResponse>> ExecuteAsync(CreateInstructorCommand command, CancellationToken cancellationToken)
    {
        var instructor = Instructor.Create(command.FullName, command.Email);
        if (instructor.IsFailure)
        {
            return instructor.Error!;
        }

        var existingInstructor = await instructorRepository.FindByNameAsync(instructor.Value!.FullName, cancellationToken);
        if (existingInstructor is not null)
        {
            return InstructorFailures.NameTaken(existingInstructor.Id);
        }

        instructorRepository.Add(instructor.Value);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            var winnerInstructor = await instructorRepository.FindByNameAsync(instructor.Value.FullName, cancellationToken);
            return InstructorFailures.NameTaken(winnerInstructor?.Id);
        }

        return InstructorResponse.From(instructor.Value);
    }
}
