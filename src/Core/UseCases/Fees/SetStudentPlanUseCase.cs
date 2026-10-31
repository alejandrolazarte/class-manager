using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record SetStudentPlanRequest(long? ClassPackId, decimal? CustomPrice, string? EffectiveFrom)
{
    public SetStudentPlanCommand ToCommand(long studentId) => new(studentId, ClassPackId, CustomPrice, EffectiveFrom);
}

public sealed record SetStudentPlanCommand(
    long StudentId,
    long? ClassPackId,
    decimal? CustomPrice,
    [property: AuditPlain] string? EffectiveFrom) : ICommand;

public sealed class SetStudentPlanUseCase(
    IStudentRepository studentRepository,
    IClientRepository clientRepository,
    IStudentPlanRepository studentPlanRepository,
    IClassPackRepository classPackRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    IAccessScopes accessScopes)
    : IUseCase<SetStudentPlanCommand, FamilyPlansResponse>
{
    public async Task<Result<FamilyPlansResponse>> ExecuteAsync(SetStudentPlanCommand command, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetSummaryByIdAsync(command.StudentId, cancellationToken);
        if (student is null)
        {
            return StudentPlanFailures.StudentNotFound();
        }

        if (!await MoneyRules.CanCollectFromAsync(accessScopes, clientRepository, student.ClientId, cancellationToken))
        {
            return AccessRules.NotYours();
        }

        var plan = command.ClassPackId is { } planId ? await classPackRepository.GetByIdAsync(planId, cancellationToken) : null;
        if (command.ClassPackId is not null && plan is not { IsActive: true, RenewsMonthly: true })
        {
            return StudentPlanFailures.PlanNotOffered(nameof(SetStudentPlanCommand.ClassPackId));
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var effectiveFrom = EffectiveMonth.Resolve(command.EffectiveFrom, today, nameof(SetStudentPlanCommand.EffectiveFrom));
        if (effectiveFrom.IsFailure)
        {
            return effectiveFrom.Error!;
        }

        var change = StudentPlanChange.Create(student.Id, effectiveFrom.Value!, plan, command.CustomPrice, today, timeProvider.GetUtcNow());
        if (change.IsFailure)
        {
            return change.Error!;
        }

        var existingChange = await studentPlanRepository.FindForUpdateAsync(student.Id, change.Value!.EffectiveFrom, cancellationToken);
        if (existingChange is null)
        {
            studentPlanRepository.Add(change.Value);
        }
        else
        {
            existingChange.ReplaceWith(change.Value);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await FamilyPlansReader.ReadAsync(student.ClientId, today, studentRepository, studentPlanRepository, classPackRepository, cancellationToken);
    }
}
