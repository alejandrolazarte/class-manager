using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Fees;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Core.UseCases.Clients;

public sealed record ClientDetailsResponse(
    Guid Id,
    string FullName,
    string PhoneNumber,
    string? Email,
    string? Notes,
    DateTimeOffset CreatedAt,
    BillingPlanResponse BillingPlan,
    IReadOnlyList<BillingPlanChangeResponse> BillingPlanChanges,
    IReadOnlyList<StudentResponse> Students)
{
    public static ClientDetailsResponse From(
        Client client,
        IEnumerable<Student> students,
        IReadOnlyCollection<ClientBillingPlanChange> billingPlanChanges,
        DateOnly today) =>
        new(
            client.Id,
            client.FullName,
            client.PhoneNumber.Value,
            client.Email,
            client.Notes,
            client.CreatedAt,
            BillingPlanResponse.From(FeeTimeline.PlanIn(billingPlanChanges, BillingMonth.From(today))),
            BillingPlanChangeResponse.From(billingPlanChanges),
            [.. students.OrderBy(student => student.FullName, StringComparer.CurrentCultureIgnoreCase).Select(StudentResponse.From)]);
}
