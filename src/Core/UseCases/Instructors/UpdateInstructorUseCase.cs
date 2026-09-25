using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record UpdateInstructorRequest(string? FullName)
{
    public UpdateInstructorCommand ToCommand(Guid instructorId) => new(instructorId, FullName);
}

public sealed record UpdateInstructorCommand(Guid InstructorId, string? FullName);

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

        var rename = instructor.Rename(command.FullName);
        if (rename.IsFailure)
        {
            return rename.Error!;
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
