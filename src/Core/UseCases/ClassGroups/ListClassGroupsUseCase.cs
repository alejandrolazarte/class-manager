using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record ListClassGroupsQuery(bool IncludeInactive) : IQuery;

public sealed class ListClassGroupsUseCase(
    IInstructorRepository instructorRepository,
    IClassGroupRepository classGroupRepository,
    IEnrollmentRepository enrollmentRepository,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes,
    IDocumentStorageService documentStorage)
    : IUseCase<ListClassGroupsQuery, IReadOnlyList<ClassGroupResponse>>
{
    public async Task<Result<IReadOnlyList<ClassGroupResponse>>> ExecuteAsync(ListClassGroupsQuery command, CancellationToken cancellationToken)
    {
        var scope = await accessScopes.ForInstructorsAsync(Permissions.ClassGroups.ViewAll, cancellationToken);
        var classGroups = (command.IncludeInactive
            ? await classGroupRepository.ListAllAsync(cancellationToken)
            : await classGroupRepository.ListActiveAsync(cancellationToken))
            .Where(classGroup => scope.Includes(classGroup.InstructorId));
        var instructorNames = (await instructorRepository.ListAllAsync(cancellationToken))
            .ToDictionary(instructor => instructor.Id, instructor => instructor.FullName);
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var enrolledCounts = await enrollmentRepository.CountCurrentByClassGroupAsync(today, cancellationToken);

        return Result.Success<IReadOnlyList<ClassGroupResponse>>(
        [
            .. classGroups
                .OrderBy(classGroup => classGroup.StartTime)
                .ThenBy(classGroup => classGroup.Name, StringComparer.CurrentCultureIgnoreCase)
                .Select(classGroup => ClassGroupResponse.From(
                    classGroup,
                    instructorNames.GetValueOrDefault(classGroup.InstructorId, string.Empty),
                    enrolledCounts.GetValueOrDefault(classGroup.Id),
                    documentStorage)),
        ]);
    }
}
