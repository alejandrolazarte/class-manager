using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record GetClassGroupQuery(Guid ClassGroupId);

public sealed class GetClassGroupUseCase(
    IInstructorRepository instructorRepository,
    IClassGroupRepository classGroupRepository,
    IEnrollmentRepository enrollmentRepository,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<GetClassGroupQuery, ClassGroupResponse>
{
    public async Task<Result<ClassGroupResponse>> ExecuteAsync(GetClassGroupQuery command, CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetByIdAsync(command.ClassGroupId, cancellationToken);
        var scope = await accessScopes.ForInstructorsAsync(Permissions.ClassGroups.ViewAll, cancellationToken);
        if (classGroup is null || !scope.Includes(classGroup.InstructorId))
        {
            return ClassGroupRules.NotFound();
        }

        var instructor = await instructorRepository.GetByIdAsync(classGroup.InstructorId, cancellationToken);
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var enrolledCount = await enrollmentRepository.CountCurrentAsync(classGroup.Id, today, cancellationToken);

        return ClassGroupResponse.From(classGroup, instructor?.FullName ?? string.Empty, enrolledCount);
    }
}
