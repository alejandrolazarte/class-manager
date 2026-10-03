using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.UseCases.ClassPacks;

internal static class ClassPackClassGroups
{
    public static async Task<Result> CoverAsync(
        ClassPack classPack,
        IReadOnlyList<Guid>? classGroupIds,
        string fieldName,
        IClassGroupRepository classGroupRepository,
        CancellationToken cancellationToken)
    {
        var requestedIds = (classGroupIds ?? []).Distinct().ToList();
        foreach (var classGroupId in requestedIds)
        {
            if (await classGroupRepository.GetByIdAsync(classGroupId, cancellationToken) is null)
            {
                return Result.Failure(ClassPackFailures.ClassGroupNotFound(fieldName));
            }
        }

        classPack.CoverClassGroups(requestedIds);
        return Result.Success();
    }
}
