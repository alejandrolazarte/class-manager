using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record ListMonthlyFeesQuery(string? Month);

public sealed record ClientFeeResponse(
    Guid ClientId,
    string ClientFullName,
    string ClientPhoneNumber,
    IReadOnlyList<string> StudentNames,
    decimal? Fee,
    decimal Paid,
    decimal Balance,
    FeeStatus Status);

public sealed record MonthlyFeesResponse(
    string Month,
    decimal TotalDue,
    decimal TotalPaid,
    IReadOnlyList<ClientFeeResponse> Clients);

public sealed class ListMonthlyFeesUseCase(
    IBusinessRepository businessRepository,
    IEnrollmentRepository enrollmentRepository,
    IPaymentRepository paymentRepository,
    IBusinessCalendarService businessCalendar)
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

        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        var enrolledStudents = await enrollmentRepository.ListEnrolledInPeriodAsync(month.Value!.FirstDay, month.Value.LastDay, cancellationToken);
        var paidByClient = await paymentRepository.SumByClientForMonthAsync(month.Value.FirstDay, cancellationToken);

        var clients = enrolledStudents
            .GroupBy(student => student.ClientId)
            .Select(clientStudents =>
            {
                var client = clientStudents.First();
                var fee = client.ClientMonthlyFee ?? business?.DefaultMonthlyFee;
                var paid = paidByClient.GetValueOrDefault(client.ClientId);
                return new ClientFeeResponse(
                    client.ClientId,
                    client.ClientFullName,
                    client.ClientPhoneNumber,
                    [.. clientStudents.Select(student => student.StudentFullName).Distinct().Order(StringComparer.CurrentCultureIgnoreCase)],
                    fee,
                    paid,
                    fee is null ? 0 : Math.Max(fee.Value - paid, 0),
                    StatusOf(fee, paid));
            })
            .OrderBy(client => client.Status)
            .ThenBy(client => client.ClientFullName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        return new MonthlyFeesResponse(
            month.Value.ToString(),
            clients.Sum(client => client.Fee ?? 0),
            clients.Sum(client => client.Paid),
            clients);
    }

    private static FeeStatus StatusOf(decimal? fee, decimal paid) => fee switch
    {
        null => FeeStatus.NoFee,
        _ when paid >= fee => FeeStatus.Paid,
        _ when paid > 0 => FeeStatus.Partial,
        _ => FeeStatus.Unpaid,
    };
}
