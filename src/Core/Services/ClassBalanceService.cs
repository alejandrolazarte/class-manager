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
    IMonthlyPlansReader monthlyPlansReader,
    IPaymentRepository paymentRepository,
    IBusinessCalendarService businessCalendar)
    : IClassBalanceService
{
    public async Task<IReadOnlyDictionary<long, ClassBalance>> CalculateAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken) =>
        clientIds.Count == 0
            ? new Dictionary<long, ClassBalance>()
            : await CalculateAsync(clientIds, await businessCalendar.TodayAsync(cancellationToken), cancellationToken);

    public async Task<IReadOnlyDictionary<long, ClassBalance>> CalculateAsync(IReadOnlyCollection<long> clientIds, DateOnly today, CancellationToken cancellationToken)
    {
        if (clientIds.Count == 0)
        {
            return new Dictionary<long, ClassBalance>();
        }

        var purchases = await purchaseRepository.ListByClientsAsync(clientIds, cancellationToken);
        IReadOnlyList<ClientAttendedClass> attendedClasses =
        [
            .. await attendanceRepository.ListAttendedClassesByClientsAsync(clientIds, cancellationToken),
            .. await privateLessonRepository.ListAttendedClassesByClientsAsync(clientIds, cancellationToken),
        ];
        var monthlyPlans = await monthlyPlansReader.ReadAsync(clientIds, cancellationToken);
        var paidMonths = (await paymentRepository.ListPaidMonthsByClientsAsync(clientIds, cancellationToken)).ToHashSet();

        return clientIds.Distinct().ToDictionary(
            clientId => clientId,
            clientId =>
            {
                var familyPlanChanges = monthlyPlans.OfFamily(clientId).ToList();
                var familyAttendedClasses = attendedClasses
                    .Where(attended => attended.ClientId == clientId)
                    .Select(attended => attended.AttendedClass)
                    .ToList();
                var notCoveredByPlans = PlanCoverage.NotCoveredByPlans(
                    familyAttendedClasses.Where(attended => !attended.IsPackBooking),
                    familyPlanChanges,
                    monthlyPlans.PlansById);
                var classesPaidPerClass = familyAttendedClasses
                    .Where(attended => attended.IsPackBooking)
                    .Concat(notCoveredByPlans.Where(attended => !IsCoveredByPaidMonth(clientId, attended, familyPlanChanges, paidMonths)))
                    .ToList();
                return ClassBalance.Calculate([.. purchases.Where(purchase => purchase.ClientId == clientId)], classesPaidPerClass, today);
            });
    }

    private static bool IsCoveredByPaidMonth(
        long clientId,
        AttendedClass attendedClass,
        IReadOnlyCollection<StudentPlanChange> familyPlanChanges,
        HashSet<ClientPaidMonth> paidMonths)
    {
        var month = BillingMonth.From(attendedClass.Date);
        var studentHasPlan = attendedClass.StudentId is { } studentId
            && PlanCoverage.HasPlanIn(familyPlanChanges.Where(change => change.StudentId == studentId), month);
        return !studentHasPlan && paidMonths.Contains(new ClientPaidMonth(clientId, month.FirstDay));
    }
}
