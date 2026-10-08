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
        var instructors = await fixture.SeedInstructorScenarioAsync();
        var owner = instructors.Business.HttpClient;
        string[] permissions =
        [
            .. SystemRolePermissions.Of(BusinessRole.Instructor),
            Permissions.Orders.ViewOwn,
            Permissions.Orders.Manage,
        ];
        var role = await fixture.SeedCustomRoleAsync(instructors.Business.Business.Id, permissions);
        var orderCollector = await fixture.SeedMemberAsync(instructors.Business.Business.Id, MemberRole.Custom(role), instructors.OtherInstructorId);
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 10);
        var fees = await owner.GetMonthlyFeesAsync();

        return new OrderCollectorScenario(
            owner,
            orderCollector,
            fees!.Clients.Single(client => client.StudentNames.Contains(InstructorScenario.OtherStudentFullName)).ClientId,
            fees.Clients.Single(client => client.StudentNames.Contains(InstructorScenario.InstructorStudentFullName)).ClientId,
            product.Variants[0].Id);
    }
}
