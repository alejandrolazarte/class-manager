using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record ChooseStudentAppPlanRequest(long? ClassPackId)
{
    public ChooseStudentAppPlanCommand ToCommand(long studentId) => new(studentId, ClassPackId);
}

public sealed record ChooseStudentAppPlanCommand(long StudentId, long? ClassPackId) : ICommand;

public sealed class ChooseStudentAppPlanUseCase(
    IStudentAppFamilyService studentAppFamily,
    IStudentPlanRepository studentPlanRepository,
    IClassPackRepository classPackRepository,
    IUnitOfWork unitOfWork,
    ITeamNotificationService teamNotifications,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<ChooseStudentAppPlanCommand, bool>
{
    private const int NextMonth = 1;

    public async Task<Result<bool>> ExecuteAsync(ChooseStudentAppPlanCommand command, CancellationToken cancellationToken)
    {
        var family = await studentAppFamily.GetAsync(cancellationToken);
        if (family is null)
        {
            return StudentAppFailures.NoAccess();
        }

        var studentError = await AccountStudentRules.FindOwnStudentErrorAsync(studentAppFamily, command.StudentId, cancellationToken);
        if (studentError is not null)
        {
            return studentError;
        }

        if (!family.CanBuy)
        {
            return StudentAppFailures.BuyerMustBeAdult();
        }

        var plan = command.ClassPackId is { } planId ? await classPackRepository.GetByIdAsync(planId, cancellationToken) : null;
        if (command.ClassPackId is not null && plan is not { RenewsMonthly: true, IsOfferedInApp: true })
        {
            return ClassPackFailures.NotFound();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var currentMonth = BillingMonth.From(today);
        var studentChanges = await studentPlanRepository.ListForUpdateAsync(command.StudentId, cancellationToken);
        var hasPlanThisMonth = StudentPlanTimeline.ChangeIn(studentChanges, currentMonth)?.HasPlan == true;
        var effectiveFrom = hasPlanThisMonth ? currentMonth.AddMonths(NextMonth) : currentMonth;
        var planInEffectBefore = StudentPlanTimeline.ChangeIn(studentChanges, effectiveFrom)?.ClassPackId;

        foreach (var laterChange in studentChanges.Where(change => change.EffectiveFrom > effectiveFrom.FirstDay))
        {
            studentPlanRepository.Remove(laterChange);
        }

        var planBefore = StudentPlanTimeline.ChangeIn(
            studentChanges.Where(change => change.EffectiveFrom < effectiveFrom.FirstDay),
            effectiveFrom)?.ClassPackId;
        var existingChange = studentChanges.FirstOrDefault(change => change.EffectiveFrom == effectiveFrom.FirstDay);
        if (planBefore == plan?.Id)
        {
            if (existingChange is not null)
            {
                studentPlanRepository.Remove(existingChange);
            }
        }
        else
        {
            var change = StudentPlanChange.Create(command.StudentId, effectiveFrom, plan, null, today, timeProvider.GetUtcNow());
            if (change.IsFailure)
            {
                return change.Error!;
            }

            if (existingChange is null)
            {
                studentPlanRepository.Add(change.Value!);
            }
            else
            {
                existingChange.ReplaceWith(change.Value!);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        if (planInEffectBefore != plan?.Id)
        {
            await teamNotifications.StudentPlanChangedAsync(command.StudentId, plan, effectiveFrom.FirstDay, cancellationToken);
        }

        return true;
    }
}
