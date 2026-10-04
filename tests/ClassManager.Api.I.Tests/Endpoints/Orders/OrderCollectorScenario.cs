using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Orders;

internal sealed record OrderCollectorScenario(
    HttpClient Owner,
    HttpClient OrderCollector,
    Guid OwnClientId,
    Guid OtherClientId,
    Guid ProductVariantId);

internal static class OrderCollectorScenarioRequests
{
    public static async Task<OrderCollectorScenario> SeedOrderCollectorScenarioAsync(this ApiFixture fixture)
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var owner = coaches.Business.HttpClient;
        string[] permissions =
        [
            .. SystemRolePermissions.Of(BusinessRole.Coach),
            Permissions.Orders.ViewOwn,
            Permissions.Orders.Manage,
        ];
        var role = await fixture.SeedCustomRoleAsync(coaches.Business.Business.Id, permissions);
        var orderCollector = await fixture.SeedMemberAsync(coaches.Business.Business.Id, MemberRole.Custom(role), coaches.OtherInstructorId);
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 10);
        var fees = await owner.GetMonthlyFeesAsync();

        return new OrderCollectorScenario(
            owner,
            orderCollector,
            fees!.Clients.Single(client => client.StudentNames.Contains(CoachScenario.OtherStudentFullName)).ClientId,
            fees.Clients.Single(client => client.StudentNames.Contains(CoachScenario.CoachStudentFullName)).ClientId,
            product.Variants[0].Id);
    }
}
