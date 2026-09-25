using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record CreateClassGroupCommand(ClassGroupDetails Details);

public sealed class CreateClassGroupUseCase(
    IInstructorRepository instructorRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<CreateClassGroupCommand, ClassGroupResponse>
{
    public async Task<Result<ClassGroupResponse>> ExecuteAsync(CreateClassGroupCommand command, CancellationToken cancellationToken)
    {
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

        var classGroup = ClassGroup.Create(details.Name, instructor.Value!.Id, schedule.Value!, details.Capacity, details.Location);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        var conflict = await ClassGroupRules.FindInstructorConflictAsync(
            classGroupRepository, instructor.Value.Id, classGroup.Value!.Id, schedule.Value!, cancellationToken);
        if (conflict is not null)
        {
            return conflict;
        }

        classGroupRepository.Add(classGroup.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClassGroupResponse.From(classGroup.Value, instructor.Value.FullName);
    }
}
