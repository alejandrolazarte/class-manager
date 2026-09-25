using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record SetClassGroupActiveCommand(Guid ClassGroupId, bool IsActive);

public sealed class SetClassGroupActiveUseCase(
    IInstructorRepository instructorRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<SetClassGroupActiveCommand, ClassGroupResponse>
{
    public async Task<Result<ClassGroupResponse>> ExecuteAsync(SetClassGroupActiveCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetForUpdateAsync(command.ClassGroupId, cancellationToken);
        if (classGroup is null)
        {
            return ClassGroupRules.NotFound();
        }

        if (!command.IsActive)
        {
            classGroup.Deactivate();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            var instructor = await instructorRepository.GetByIdAsync(classGroup.InstructorId, cancellationToken);
            return ClassGroupResponse.From(classGroup, instructor?.FullName ?? string.Empty);
        }

        var activeInstructor = await ClassGroupRules.FindActiveInstructorAsync(instructorRepository, classGroup.InstructorId, cancellationToken);
        if (activeInstructor.IsFailure)
        {
            return activeInstructor.Error!;
        }

        var conflict = await ClassGroupRules.FindInstructorConflictAsync(
            classGroupRepository, classGroup.InstructorId, classGroup.Id, classGroup.Schedule, cancellationToken);
        if (conflict is not null)
        {
            return conflict;
        }

        classGroup.Activate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClassGroupResponse.From(classGroup, activeInstructor.Value!.FullName);
    }
}
