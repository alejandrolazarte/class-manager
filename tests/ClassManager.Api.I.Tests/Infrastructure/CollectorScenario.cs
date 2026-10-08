using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record CollectorScenario(InstructorScenario Instructors, HttpClient Collector, Guid CollectorClientId, Guid OtherClientId);

public static class CollectorScenarioRequests
{
    public static async Task<CollectorScenario> SeedCollectorScenarioAsync(this ApiFixture fixture)
    {
        var instructors = await fixture.SeedInstructorScenarioAsync();
        string[] permissions =
        [
            .. SystemRolePermissions.Of(BusinessRole.Instructor),
            Permissions.Payments.ViewOwn,
            Permissions.Payments.Record,
            Permissions.ClassPacks.Sell,
        ];
        var role = await fixture.SeedCustomRoleAsync(instructors.Business.Business.Id, permissions);
        var collector = await fixture.SeedMemberAsync(instructors.Business.Business.Id, MemberRole.Custom(role), instructors.OtherInstructorId);
        var fees = await instructors.Business.HttpClient.GetMonthlyFeesAsync();

        return new CollectorScenario(
            instructors,
            collector,
            ClientOf(fees!, InstructorScenario.OtherStudentFullName),
            ClientOf(fees!, InstructorScenario.InstructorStudentFullName));
    }

    public static Task<List<PaymentResponse>?> GetClientPaymentsAsync(this HttpClient httpClient, Guid clientId) =>
        httpClient.GetFromJsonAsync<List<PaymentResponse>>(
            new Uri($"{ApiRoutes.Clients}/{clientId}{ApiRoutes.PaymentsSegment}", UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeletePaymentAsync(this HttpClient httpClient, Guid paymentId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Payments}/{paymentId}", UriKind.Relative));

    public static Task<HttpResponseMessage> RestorePaymentAsync(this HttpClient httpClient, Guid paymentId) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.Payments}/{paymentId}{ApiRoutes.Restore}", UriKind.Relative), null);

    private static Guid ClientOf(MonthlyFeesResponse fees, string studentFullName) =>
        fees.Clients.Single(client => client.StudentNames.Contains(studentFullName)).ClientId;
}
