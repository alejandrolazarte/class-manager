using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record UpdateClassPackRequest(
    string? Name,
    int? ClassCount,
    decimal? Price,
    int? ValidityMonths,
    int? ClassDurationMinutes = null,
    IReadOnlyList<long>? ClassGroupIds = null,
    string? Description = null,
    bool CoversAnyGroupClass = false)
{
    public UpdateClassPackCommand ToCommand(long classPackId) =>
        new(classPackId, Name, ClassCount, Price, ValidityMonths, ClassDurationMinutes, ClassGroupIds, Description, CoversAnyGroupClass);
}

public sealed record UpdateClassPackCommand(
    long ClassPackId,
    [property: AuditPlain] string? Name,
    int? ClassCount,
    decimal? Price,
    int? ValidityMonths,
    int? ClassDurationMinutes = null,
    IReadOnlyList<long>? ClassGroupIds = null,
    [property: AuditPlain] string? Description = null,
    bool CoversAnyGroupClass = false) : ICommand;

public sealed class UpdateClassPackUseCase(
    IClassPackRepository classPackRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork,
    IDocumentStorageService documentStorage)
    : IUseCase<UpdateClassPackCommand, ClassPackResponse>
{
    public async Task<Result<ClassPackResponse>> ExecuteAsync(UpdateClassPackCommand command, CancellationToken cancellationToken)
    {
        var classPack = await classPackRepository.GetForUpdateAsync(command.ClassPackId, cancellationToken);
        if (classPack is null)
        {
            return ClassPackFailures.NotFound();
        }

        var update = classPack.Update(command.Name, command.ClassCount, command.Price, command.ValidityMonths);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        var description = classPack.Describe(command.Description);
        if (description.IsFailure)
        {
            return description.Error!;
        }

        var lessons = await ClassPackClassGroups.DefineLessonsAsync(
            classPack,
            command.ClassGroupIds,
            command.ClassDurationMinutes,
            command.CoversAnyGroupClass,
            nameof(UpdateClassPackCommand.ClassGroupIds),
            classGroupRepository,
            cancellationToken);
        if (lessons.IsFailure)
        {
            return lessons.Error!;
        }

        var packWithSameName = await classPackRepository.FindByNameAsync(classPack.Name, cancellationToken);
        if (packWithSameName is not null && packWithSameName.Id != classPack.Id)
        {
            return ClassPackFailures.NameTaken();
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return ClassPackFailures.NameTaken();
        }

        return ClassPackResponse.From(classPack, documentStorage);
    }
}
