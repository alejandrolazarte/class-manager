using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record UpdateClassGroupCommand(Guid ClassGroupId, ClassGroupDetails Details);

public sealed class UpdateClassGroupUseCase(
    IInstructorRepository instructorRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<UpdateClassGroupCommand, ClassGroupResponse>
{
    public async Task<Result<ClassGroupResponse>> ExecuteAsync(UpdateClassGroupCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetForUpdateAsync(command.ClassGroupId, cancellationToken);
        if (classGroup is null)
        {
            return ClassGroupRules.NotFound();
        }

        var details = command.Details;
        var schedule = ClassSchedule.Create(details.Weekdays, details.StartTime, details.DurationMinutes);
        if (schedule.IsFailure)
        {
            return schedule.Error!;
        }

        var instructor = await ClassGroupRules.FindActiveInstructorAsync(instructorRepository, details.InstructorId, cancellationToken);
        if (instructor.IsFailure)
        {
            return instructor.Error!;
        }

        if (classGroup.IsActive)
        {
            var conflict = await ClassGroupRules.FindInstructorConflictAsync(
                classGroupRepository, instructor.Value!.Id, classGroup.Id, schedule.Value!, cancellationToken);
            if (conflict is not null)
            {
                return conflict;
            }
        }

        var update = classGroup.Update(details.Name, instructor.Value!.Id, schedule.Value!, details.Capacity, details.Location);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClassGroupResponse.From(classGroup, instructor.Value.FullName);
    }
}
