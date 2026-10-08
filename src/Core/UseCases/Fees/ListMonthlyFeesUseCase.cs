using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record ListMonthlyFeesQuery(string? Month) : IQuery;

public sealed record ClientFeeResponse(
    Guid ClientId,
    string ClientFullName,
    string ClientPhoneNumber,
    IReadOnlyList<string> StudentNames,
    decimal? Fee,
    decimal Paid,
    decimal Balance,
    FeeStatus Status);

public sealed record ClassPackClientResponse(
    Guid ClientId,
    string ClientFullName,
    string ClientPhoneNumber,
    IReadOnlyList<string> StudentNames,
    int AvailableClasses,
    int UnpaidClasses);

public sealed record MonthlyFeesResponse(
    string Month,
    decimal TotalDue,
    decimal TotalPaid,
    IReadOnlyList<ClientFeeResponse> Clients,
    IReadOnlyList<ClassPackClientResponse> ClassPackClients);

public sealed class ListMonthlyFeesUseCase(
    IEnrollmentRepository enrollmentRepository,
    IPaymentRepository paymentRepository,
    IFeeScheduleRepository feeScheduleRepository,
    IClassBalanceService classBalanceService,
    IClientRepository clientRepository,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<ListMonthlyFeesQuery, MonthlyFeesResponse>
{
    public async Task<Result<MonthlyFeesResponse>> ExecuteAsync(ListMonthlyFeesQuery command, CancellationToken cancellationToken)
    {
        var month = command.Month is null
            ? BillingMonth.From(await businessCalendar.TodayAsync(cancellationToken))
            : BillingMonth.Parse(command.Month, nameof(ListMonthlyFeesQuery.Month));
        if (month.IsFailure)
        {
            return month.Error!;
        }

        var scope = await accessScopes.ForClientsAsync(Permissions.Payments.ViewAll, cancellationToken);
        var clientIdsInScope = scope is null ? null : (await clientRepository.ListIdsInScopeAsync(scope, cancellationToken)).ToHashSet();
        var enrolledStudents = await enrollmentRepository.ListEnrolledInPeriodAsync(month.Value!.FirstDay, month.Value.LastDay, cancellationToken);
        var enrolledClients = enrolledStudents
            .Where(student => clientIdsInScope is null || clientIdsInScope.Contains(student.ClientId))
            .GroupBy(student => student.ClientId)
            .Select(clientStudents => new EnrolledClient(
                clientStudents.First(),
                [.. clientStudents.Select(student => student.StudentFullName).Distinct().Order(StringComparer.CurrentCultureIgnoreCase)]))
            .ToList();
        var clientIds = enrolledClients.Select(enrolledClient => enrolledClient.Row.ClientId).ToList();

        var defaultFee = FeeTimeline.DefaultFeeIn(await feeScheduleRepository.ListDefaultFeeChangesAsync(cancellationToken), month.Value);
        var planChanges = await feeScheduleRepository.ListClientPlanChangesAsync(clientIds, cancellationToken);
        var planByClient = enrolledClients.ToDictionary(
            enrolledClient => enrolledClient.Row.ClientId,
            enrolledClient => FeeTimeline.PlanIn(planChanges.Where(change => change.ClientId == enrolledClient.Row.ClientId), month.Value));

        var paidByClient = await paymentRepository.SumByClientForMonthAsync(month.Value.FirstDay, cancellationToken);
        var monthlyClients = ListMonthlyClients(
            enrolledClients.Where(enrolledClient => !planByClient[enrolledClient.Row.ClientId].PaysPerClass).ToList(),
            planByClient,
            defaultFee,
            paidByClient);
        var classPackClients = await ListClassPackClientsAsync(
            enrolledClients.Where(enrolledClient => planByClient[enrolledClient.Row.ClientId].PaysPerClass).ToList(),
            cancellationToken);

        return new MonthlyFeesResponse(
            month.Value.ToString(),
            monthlyClients.Sum(client => client.Fee ?? 0),
            clientIds.Sum(clientId => paidByClient.GetValueOrDefault(clientId)),
            monthlyClients,
            classPackClients);
    }

    private static IReadOnlyList<ClientFeeResponse> ListMonthlyClients(
        IReadOnlyList<EnrolledClient> enrolledClients,
        Dictionary<Guid, BillingPlan> planByClient,
        decimal? defaultFee,
        IReadOnlyDictionary<Guid, decimal> paidByClient)
    {
        return [.. enrolledClients
            .Select(enrolledClient =>
            {
                var clientId = enrolledClient.Row.ClientId;
                var fee = planByClient[clientId].MonthlyFee(defaultFee);
                var paid = paidByClient.GetValueOrDefault(clientId);
                return new ClientFeeResponse(
                    clientId,
                    enrolledClient.Row.ClientFullName,
                    enrolledClient.Row.ClientPhoneNumber,
                    enrolledClient.StudentNames,
                    fee,
                    paid,
                    fee is null ? 0 : Math.Max(fee.Value - paid, 0),
                    FeeRules.StatusOf(fee, paid));
            })
            .OrderBy(client => client.Status)
            .ThenBy(client => client.ClientFullName, StringComparer.CurrentCultureIgnoreCase)];
    }

    private async Task<IReadOnlyList<ClassPackClientResponse>> ListClassPackClientsAsync(
        IReadOnlyList<EnrolledClient> enrolledClients,
        CancellationToken cancellationToken)
    {
        var balances = await classBalanceService.CalculateAsync([.. enrolledClients.Select(enrolledClient => enrolledClient.Row.ClientId)], cancellationToken);

        return [.. enrolledClients
            .Select(enrolledClient =>
            {
                var balance = balances[enrolledClient.Row.ClientId];
                return new ClassPackClientResponse(
                    enrolledClient.Row.ClientId,
                    enrolledClient.Row.ClientFullName,
                    enrolledClient.Row.ClientPhoneNumber,
                    enrolledClient.StudentNames,
                    balance.AvailableClasses,
                    balance.UnpaidClasses);
            })
            .OrderByDescending(client => client.UnpaidClasses)
            .ThenBy(client => client.AvailableClasses)
            .ThenBy(client => client.ClientFullName, StringComparer.CurrentCultureIgnoreCase)];
    }

    private sealed record EnrolledClient(EnrolledStudentInPeriod Row, IReadOnlyList<string> StudentNames);
}
