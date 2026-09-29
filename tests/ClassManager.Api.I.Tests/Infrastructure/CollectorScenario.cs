using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record CollectorScenario(CoachScenario Coaches, HttpClient Collector, Guid CollectorFamilyId, Guid OtherFamilyId);

public static class CollectorScenarioRequests
{
    public static async Task<CollectorScenario> SeedCollectorScenarioAsync(this ApiFixture fixture)
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        string[] permissions =
        [
            .. SystemRolePermissions.Of(BusinessRole.Coach),
            Permissions.Payments.ViewOwn,
            Permissions.Payments.Record,
            Permissions.ClassPacks.Sell,
        ];
        var role = await fixture.SeedCustomRoleAsync(coaches.Business.Business.Id, permissions);
        var collector = await fixture.SeedMemberAsync(coaches.Business.Business.Id, MemberRole.Custom(role), coaches.OtherInstructorId);
        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();

        return new CollectorScenario(
            coaches,
            collector,
            FamilyOf(fees!, CoachScenario.OtherStudentFullName),
            FamilyOf(fees!, CoachScenario.CoachStudentFullName));
    }

    public static Task<List<PaymentResponse>?> GetClientPaymentsAsync(this HttpClient httpClient, Guid clientId) =>
        httpClient.GetFromJsonAsync<List<PaymentResponse>>(
            new Uri($"{ApiRoutes.Clients}/{clientId}{ApiRoutes.PaymentsSegment}", UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeletePaymentAsync(this HttpClient httpClient, Guid paymentId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Payments}/{paymentId}", UriKind.Relative));

    private static Guid FamilyOf(MonthlyFeesResponse fees, string studentFullName) =>
        fees.Clients.Single(client => client.StudentNames.Contains(studentFullName)).ClientId;
}
