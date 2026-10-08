using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Services;

public sealed class ClassBalanceService(
    IClassPackPurchaseRepository purchaseRepository,
    IAttendanceRepository attendanceRepository,
    IPrivateLessonRepository privateLessonRepository,
    IFeeScheduleRepository feeScheduleRepository,
    IPaymentRepository paymentRepository,
    IBusinessCalendarService businessCalendar)
    : IClassBalanceService
{
    public async Task<IReadOnlyDictionary<Guid, ClassBalance>> CalculateAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken)
    {
        if (clientIds.Count == 0)
        {
            return new Dictionary<Guid, ClassBalance>();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var purchases = await purchaseRepository.ListByClientsAsync(clientIds, cancellationToken);
        IReadOnlyList<ClientAttendedClass> attendedClasses =
        [
            .. await attendanceRepository.ListAttendedClassesByClientsAsync(clientIds, cancellationToken),
            .. await privateLessonRepository.ListAttendedClassesByClientsAsync(clientIds, cancellationToken),
        ];
        var planChanges = await feeScheduleRepository.ListClientPlanChangesAsync(clientIds, cancellationToken);
        var paidMonths = (await paymentRepository.ListPaidMonthsByClientsAsync(clientIds, cancellationToken)).ToHashSet();

        return clientIds.Distinct().ToDictionary(
            clientId => clientId,
            clientId =>
            {
                var clientPlanChanges = planChanges.Where(change => change.ClientId == clientId).ToList();
                var classesPaidPerClass = attendedClasses
                    .Where(attended => attended.ClientId == clientId)
                    .Select(attended => attended.AttendedClass)
                    .Where(attended => attended.IsPackBooking || IsPaidPerClass(clientId, attended.Date, clientPlanChanges, paidMonths))
                    .ToList();
                return ClassBalance.Calculate([.. purchases.Where(purchase => purchase.ClientId == clientId)], classesPaidPerClass, today);
            });
    }

    private static bool IsPaidPerClass(
        Guid clientId,
        DateOnly attendedOn,
        List<ClientBillingPlanChange> clientPlanChanges,
        HashSet<ClientPaidMonth> paidMonths)
    {
        var month = BillingMonth.From(attendedOn);
        var isCoveredByPaidFee = paidMonths.Contains(new ClientPaidMonth(clientId, month.FirstDay));
        return !isCoveredByPaidFee && FeeTimeline.PlanIn(clientPlanChanges, month).PaysPerClass;
    }
}
