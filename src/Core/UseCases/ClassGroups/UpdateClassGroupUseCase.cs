using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record UpdateClassGroupCommand(Guid ClassGroupId, ClassGroupDetails Details) : ICommand;

public sealed class UpdateClassGroupUseCase(
    IInstructorRepository instructorRepository,
    IClassGroupRepository classGroupRepository,
    IEnrollmentRepository enrollmentRepository,
    IPrivateLessonRepository privateLessonRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar)
    : IUseCase<UpdateClassGroupCommand, ClassGroupResponse>
{
    private const string CapacityBelowEnrolledMessage = "Capacity can't be lower than the students currently enrolled.";

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
                classGroupRepository, instructor.Value!.Id, classGroup.Id, schedule.Value!, cancellationToken)
                ?? await InstructorAgendaRules.FindWeeklyPrivateLessonConflictAsync(
                    privateLessonRepository, instructor.Value.Id, schedule.Value!, await businessCalendar.TodayAsync(cancellationToken), cancellationToken);
            if (conflict is not null)
            {
                return conflict;
            }
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var enrolledCount = await enrollmentRepository.CountCurrentAsync(classGroup.Id, today, cancellationToken);
        if (details.Capacity < enrolledCount)
        {
            return Result.Validation<ClassGroupResponse>(
                CapacityBelowEnrolledMessage,
                ClassGroupErrorCodes.CapacityBelowEnrolled,
                nameof(ClassGroupDetails.Capacity));
        }

        var update = classGroup.Update(details.Name, instructor.Value!.Id, schedule.Value!, details.Capacity, details.Location);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClassGroupResponse.From(classGroup, instructor.Value.FullName, enrolledCount);
    }
}
