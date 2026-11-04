using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record SellStudentPlanRequest(long? ClassPackId, decimal? PaidAmount, PaymentMethod? Method, string? Notes)
{
    public SellStudentPlanCommand ToCommand(long studentId) => new(studentId, ClassPackId, PaidAmount, Method, Notes);
}

public sealed record SellStudentPlanCommand(
    long StudentId,
    long? ClassPackId,
    decimal? PaidAmount,
    PaymentMethod? Method,
    [property: AuditPersonal] string? Notes) : ICommand;

public sealed class SellStudentPlanUseCase(
    IStudentRepository studentRepository,
    IClientRepository clientRepository,
    IStudentPlanRepository studentPlanRepository,
    IClassPackRepository classPackRepository,
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IBusinessCurrencyService businessCurrency,
    TimeProvider timeProvider,
    IAccessScopes accessScopes,
    ICurrentMember currentMember)
    : IUseCase<SellStudentPlanCommand, FamilyPlansResponse>
{
    public async Task<Result<FamilyPlansResponse>> ExecuteAsync(SellStudentPlanCommand command, CancellationToken cancellationToken)
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
        if (plan is not { IsActive: true, RenewsMonthly: true })
        {
            return StudentPlanFailures.PlanNotOffered(nameof(SellStudentPlanCommand.ClassPackId));
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var now = timeProvider.GetUtcNow();
        var currentMonth = BillingMonth.From(today);
        var change = StudentPlanChange.Create(student.Id, currentMonth, plan, null, today, now);
        if (change.IsFailure)
        {
            return change.Error!;
        }

        var studentChanges = await studentPlanRepository.ListForUpdateAsync(student.Id, cancellationToken);
        foreach (var laterChange in studentChanges.Where(existing => existing.EffectiveFrom > currentMonth.FirstDay))
        {
            studentPlanRepository.Remove(laterChange);
        }

        var existingChange = studentChanges.FirstOrDefault(existing => existing.EffectiveFrom == currentMonth.FirstDay);
        if (existingChange is null)
        {
            studentPlanRepository.Add(change.Value!);
        }
        else
        {
            existingChange.ReplaceWith(change.Value!);
        }

        if (command.PaidAmount is { } paidAmount && paidAmount != 0)
        {
            var access = await currentMember.GetAccessAsync(cancellationToken);
            var payment = Payment.Create(
                student.ClientId,
                paidAmount,
                await businessCurrency.CurrencyCodeAsync(cancellationToken),
                currentMonth,
                today,
                command.Method,
                command.Notes,
                today,
                now,
                access?.UserId);
            if (payment.IsFailure)
            {
                return payment.Error!;
            }

            paymentRepository.Add(payment.Value!);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await FamilyPlansReader.ReadAsync(student.ClientId, today, studentRepository, studentPlanRepository, classPackRepository, cancellationToken);
    }
}
