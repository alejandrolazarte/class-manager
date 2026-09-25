using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.UseCases.Enrollments;

public sealed record ListClassRosterQuery(Guid ClassGroupId);

public sealed class ListClassRosterUseCase(
    IClassGroupRepository classGroupRepository,
    IEnrollmentRepository enrollmentRepository,
    IBusinessCalendarService businessCalendar)
    : IUseCase<ListClassRosterQuery, IReadOnlyList<RosterEntryResponse>>
{
    private const string ClassGroupNotFoundMessage = "The class group does not exist.";

    public async Task<Result<IReadOnlyList<RosterEntryResponse>>> ExecuteAsync(ListClassRosterQuery command, CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetByIdAsync(command.ClassGroupId, cancellationToken);
        if (classGroup is null)
        {
            return Result.NotFound<IReadOnlyList<RosterEntryResponse>>(ClassGroupNotFoundMessage, ClassGroupErrorCodes.NotFound);
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var roster = await enrollmentRepository.ListRosterAsync(classGroup.Id, today, cancellationToken);

        return Result.Success<IReadOnlyList<RosterEntryResponse>>([.. roster.Select(RosterEntryResponse.From)]);
    }
}
