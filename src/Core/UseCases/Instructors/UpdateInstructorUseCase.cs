using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record UpdateInstructorRequest(string? FullName, string? Email = null)
{
    public UpdateInstructorCommand ToCommand(Guid instructorId) => new(instructorId, FullName, Email);
}

public sealed record UpdateInstructorCommand(Guid InstructorId, string? FullName, string? Email = null) : ICommand;

public sealed class UpdateInstructorUseCase(IInstructorRepository instructorRepository, IUnitOfWork unitOfWork)
    : IUseCase<UpdateInstructorCommand, InstructorResponse>
{
    public async Task<Result<InstructorResponse>> ExecuteAsync(UpdateInstructorCommand command, CancellationToken cancellationToken)
    {
        var instructor = await instructorRepository.GetForUpdateAsync(command.InstructorId, cancellationToken);
        if (instructor is null)
        {
            return InstructorFailures.NotFound();
        }

        var update = instructor.Update(command.FullName, command.Email);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        var existingInstructor = await instructorRepository.FindByNameAsync(instructor.FullName, cancellationToken);
        if (existingInstructor is not null && existingInstructor.Id != instructor.Id)
        {
            return InstructorFailures.NameTaken(existingInstructor.Id);
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return InstructorFailures.NameTaken(null);
        }

        return InstructorResponse.From(instructor);
    }
}
