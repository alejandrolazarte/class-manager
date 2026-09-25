using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record SetInstructorActiveCommand(Guid InstructorId, bool IsActive);

public sealed class SetInstructorActiveUseCase(
    IInstructorRepository instructorRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<SetInstructorActiveCommand, InstructorResponse>
{
    public async Task<Result<InstructorResponse>> ExecuteAsync(SetInstructorActiveCommand command, CancellationToken cancellationToken)
    {
        var instructor = await instructorRepository.GetForUpdateAsync(command.InstructorId, cancellationToken);
        if (instructor is null)
        {
            return InstructorFailures.NotFound();
        }

        if (command.IsActive)
        {
            instructor.Activate();
        }
        else
        {
            var activeClassGroupCount = await classGroupRepository.CountActiveByInstructorAsync(instructor.Id, cancellationToken);
            if (activeClassGroupCount > 0)
            {
                return InstructorFailures.HasActiveClassGroups(activeClassGroupCount);
            }

            instructor.Deactivate();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return InstructorResponse.From(instructor);
    }
}
