using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record GetClassGroupQuery(Guid ClassGroupId);

public sealed class GetClassGroupUseCase(IInstructorRepository instructorRepository, IClassGroupRepository classGroupRepository)
    : IUseCase<GetClassGroupQuery, ClassGroupResponse>
{
    public async Task<Result<ClassGroupResponse>> ExecuteAsync(GetClassGroupQuery command, CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetByIdAsync(command.ClassGroupId, cancellationToken);
        if (classGroup is null)
        {
            return ClassGroupRules.NotFound();
        }

        var instructor = await instructorRepository.GetByIdAsync(classGroup.InstructorId, cancellationToken);

        return ClassGroupResponse.From(classGroup, instructor?.FullName ?? string.Empty);
    }
}
