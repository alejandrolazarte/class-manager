using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Core.UseCases.ClassGroups;

internal static class ClassGroupRules
{
    private const string NotFoundMessage = "The class group does not exist.";
    private const string InstructorBusyMessage = "The instructor already teaches another class group at that time.";

    public static ResultError NotFound() => new(ClassGroupErrorCodes.NotFound, NotFoundMessage, ErrorKind.NotFound);

    public static async Task<Result<Instructor>> FindActiveInstructorAsync(
        IInstructorRepository instructorRepository,
        Guid? instructorId,
        CancellationToken cancellationToken)
    {
        var instructor = instructorId is null ? null : await instructorRepository.GetByIdAsync(instructorId.Value, cancellationToken);
        if (instructor is null)
        {
            return InstructorFailures.NotFound();
        }

        return instructor.IsActive ? instructor : InstructorFailures.Inactive(nameof(ClassGroupDetails.InstructorId));
    }

    public static async Task<ResultError?> FindInstructorConflictAsync(
        IClassGroupRepository classGroupRepository,
        Guid instructorId,
        Guid classGroupId,
        ClassSchedule schedule,
        CancellationToken cancellationToken)
    {
        var instructorClassGroups = await classGroupRepository.ListActiveByInstructorAsync(instructorId, cancellationToken);
        var overlappingClassGroup = instructorClassGroups.FirstOrDefault(other =>
            other.Id != classGroupId && other.Schedule.OverlapsWith(schedule));

        return overlappingClassGroup is null
            ? null
            : new ResultError(ClassGroupErrorCodes.InstructorBusy, InstructorBusyMessage, ErrorKind.Conflict)
            {
                Details = new Dictionary<string, object?> { [ClassGroupErrorCodes.ConflictingClassGroupIdDetail] = overlappingClassGroup.Id },
            };
    }
}
