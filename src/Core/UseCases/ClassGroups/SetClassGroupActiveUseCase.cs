using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record SetClassGroupActiveCommand(Guid ClassGroupId, bool IsActive) : ICommand;

public sealed class SetClassGroupActiveUseCase(
    IInstructorRepository instructorRepository,
    IClassGroupRepository classGroupRepository,
    IEnrollmentRepository enrollmentRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar)
    : IUseCase<SetClassGroupActiveCommand, ClassGroupResponse>
{
    private const string HasEnrollmentsMessage = "The class group still has enrolled students.";

    public async Task<Result<ClassGroupResponse>> ExecuteAsync(SetClassGroupActiveCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetForUpdateAsync(command.ClassGroupId, cancellationToken);
        if (classGroup is null)
        {
            return ClassGroupRules.NotFound();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var enrolledCount = await enrollmentRepository.CountCurrentAsync(classGroup.Id, today, cancellationToken);

        if (!command.IsActive)
        {
            if (enrolledCount > 0)
            {
                return Result.Conflict<ClassGroupResponse>(
                    HasEnrollmentsMessage,
                    ClassGroupErrorCodes.HasEnrollments,
                    new Dictionary<string, object?> { [ClassGroupErrorCodes.EnrollmentCountDetail] = enrolledCount });
            }

            classGroup.Deactivate();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            var instructor = await instructorRepository.GetByIdAsync(classGroup.InstructorId, cancellationToken);
            return ClassGroupResponse.From(classGroup, instructor?.FullName ?? string.Empty, enrolledCount);
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

        return ClassGroupResponse.From(classGroup, activeInstructor.Value!.FullName, enrolledCount);
    }
}
