using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Sessions;

internal static class SessionRules
{
    private const string ClassGroupNotFoundMessage = "The class group does not exist.";
    private const string NotScheduledMessage = "The class group doesn't meet on that date.";

    public static ResultError ClassGroupNotFound() => new(ClassGroupErrorCodes.NotFound, ClassGroupNotFoundMessage, ErrorKind.NotFound);

    public static async Task<Result<ClassGroup>> FindScheduledClassGroupAsync(
        IClassGroupRepository classGroupRepository,
        Guid classGroupId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetByIdAsync(classGroupId, cancellationToken);
        if (classGroup is null)
        {
            return Result.NotFound<ClassGroup>(ClassGroupNotFoundMessage, ClassGroupErrorCodes.NotFound);
        }

        return classGroup.Schedule.MeetsOn(date.DayOfWeek)
            ? classGroup
            : Result.Validation<ClassGroup>(NotScheduledMessage, SessionErrorCodes.NotScheduled, nameof(ClassSession.Date));
    }
}
