using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record ListClassGroupsQuery(bool IncludeInactive);

public sealed class ListClassGroupsUseCase(IInstructorRepository instructorRepository, IClassGroupRepository classGroupRepository)
    : IUseCase<ListClassGroupsQuery, IReadOnlyList<ClassGroupResponse>>
{
    public async Task<Result<IReadOnlyList<ClassGroupResponse>>> ExecuteAsync(ListClassGroupsQuery command, CancellationToken cancellationToken)
    {
        var classGroups = command.IncludeInactive
            ? await classGroupRepository.ListAllAsync(cancellationToken)
            : await classGroupRepository.ListActiveAsync(cancellationToken);
        var instructorNames = (await instructorRepository.ListAllAsync(cancellationToken))
            .ToDictionary(instructor => instructor.Id, instructor => instructor.FullName);

        return Result.Success<IReadOnlyList<ClassGroupResponse>>(
        [
            .. classGroups
                .OrderBy(classGroup => classGroup.StartTime)
                .ThenBy(classGroup => classGroup.Name, StringComparer.CurrentCultureIgnoreCase)
                .Select(classGroup => ClassGroupResponse.From(classGroup, instructorNames.GetValueOrDefault(classGroup.InstructorId, string.Empty))),
        ]);
    }
}
