using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record UpdateClassPackRequest(
    string? Name,
    int? ClassCount,
    decimal? Price,
    int? ValidityMonths,
    int? ClassDurationMinutes = null,
    IReadOnlyList<long>? ClassGroupIds = null,
    string? Description = null,
    bool CoversAnyGroupClass = false,
    bool RenewsMonthly = false,
    bool? AppliesFamilyDiscount = null,
    string? PriceEffectiveFrom = null)
{
    public UpdateClassPackCommand ToCommand(long classPackId) =>
        new(classPackId, Name, ClassCount, Price, ValidityMonths, ClassDurationMinutes, ClassGroupIds, Description, CoversAnyGroupClass, RenewsMonthly, AppliesFamilyDiscount, PriceEffectiveFrom);
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
    bool CoversAnyGroupClass = false,
    bool RenewsMonthly = false,
    bool? AppliesFamilyDiscount = null,
    [property: AuditPlain] string? PriceEffectiveFrom = null) : ICommand;

public sealed class UpdateClassPackUseCase(
    IClassPackRepository classPackRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
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

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var priceChanges = await classPackRepository.ListPriceChangesAsync(cancellationToken);
        var keepsRenewing = classPack.RenewsMonthly && command.RenewsMonthly;
        var priceNow = PlanPrices.PriceIn(classPack, priceChanges, BillingMonth.From(today));
        var update = classPack.Update(
            command.Name,
            command.ClassCount,
            keepsRenewing ? classPack.Price : command.Price,
            command.ValidityMonths,
            command.RenewsMonthly,
            command.AppliesFamilyDiscount);
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

        if (keepsRenewing && command.Price != priceNow)
        {
            var priceChange = await ChangePriceAsync(classPack, command, today, cancellationToken);
            if (priceChange is not null)
            {
                return priceChange;
            }
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

        return await ClassPackResponses.ForAsync(classPack, classPackRepository, businessCalendar, documentStorage, cancellationToken);
    }

    private async Task<ResultError?> ChangePriceAsync(ClassPack plan, UpdateClassPackCommand command, DateOnly today, CancellationToken cancellationToken)
    {
        var effectiveFrom = EffectiveMonth.Resolve(command.PriceEffectiveFrom, today, nameof(UpdateClassPackCommand.PriceEffectiveFrom));
        if (effectiveFrom.IsFailure)
        {
            return effectiveFrom.Error;
        }

        var change = ClassPackPriceChange.Create(plan.Id, effectiveFrom.Value!, command.Price, today, timeProvider.GetUtcNow());
        if (change.IsFailure)
        {
            return change.Error;
        }

        var existingChange = await classPackRepository.FindPriceChangeForUpdateAsync(plan.Id, change.Value!.EffectiveFrom, cancellationToken);
        if (existingChange is null)
        {
            classPackRepository.Add(change.Value);
        }
        else
        {
            existingChange.ReplaceWith(change.Value);
        }

        return null;
    }
}
