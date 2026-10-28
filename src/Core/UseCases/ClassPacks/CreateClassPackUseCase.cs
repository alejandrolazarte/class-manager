using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record CreateClassPackCommand(
    [property: AuditPlain] string? Name,
    int? ClassCount,
    decimal? Price,
    int? ValidityMonths,
    int? ClassDurationMinutes = null,
    IReadOnlyList<long>? ClassGroupIds = null,
    [property: AuditPlain] string? Description = null,
    bool CoversAnyGroupClass = false) : ICommand;

public sealed class CreateClassPackUseCase(
    IClassPackRepository classPackRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IDocumentStorageService documentStorage)
    : IUseCase<CreateClassPackCommand, ClassPackResponse>
{
    public async Task<Result<ClassPackResponse>> ExecuteAsync(CreateClassPackCommand command, CancellationToken cancellationToken)
    {
        var classPack = ClassPack.Create(command.Name, command.ClassCount, command.Price, command.ValidityMonths, timeProvider.GetUtcNow());
        if (classPack.IsFailure)
        {
            return classPack.Error!;
        }

        var description = classPack.Value!.Describe(command.Description);
        if (description.IsFailure)
        {
            return description.Error!;
        }

        var lessons = await ClassPackClassGroups.DefineLessonsAsync(
            classPack.Value,
            command.ClassGroupIds,
            command.ClassDurationMinutes,
            command.CoversAnyGroupClass,
            nameof(CreateClassPackCommand.ClassGroupIds),
            classGroupRepository,
            cancellationToken);
        if (lessons.IsFailure)
        {
            return lessons.Error!;
        }

        if (await classPackRepository.FindByNameAsync(classPack.Value!.Name, cancellationToken) is not null)
        {
            return ClassPackFailures.NameTaken();
        }

        classPackRepository.Add(classPack.Value);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return ClassPackFailures.NameTaken();
        }

        return ClassPackResponse.From(classPack.Value, documentStorage);
    }
}
