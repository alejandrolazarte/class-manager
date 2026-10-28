using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.UseCases.ClassPacks;

internal static class ClassPackClassGroups
{
    public static async Task<Result> DefineLessonsAsync(
        ClassPack classPack,
        IReadOnlyList<long>? classGroupIds,
        int? classDurationMinutes,
        bool coversAnyGroupClass,
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

        return classPack.DefineLessons(requestedIds, classDurationMinutes, coversAnyGroupClass);
    }
}
