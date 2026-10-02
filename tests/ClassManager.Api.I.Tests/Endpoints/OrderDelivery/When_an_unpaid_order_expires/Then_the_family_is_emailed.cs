using ClassManager.Api.Orders;
using ClassManager.Core.Domain.Orders;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_an_unpaid_order_expires;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_is_emailed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_family_is_emailed_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var classPack = await scenario.Coaches.Business.HttpClient.CreateClassPackAsync();
        await using (var context = fixture.CreateDbContext(scenario.Coaches.Business.Business.Id))
        {
            var pack = await context.ClassPacks.FindAsync(classPack.Id);
            context.Orders.Add(Order.Request(
                scenario.FamilyId, [OrderLine.ForClassPack(pack!, null).Value!], null, BusinessApiFactory.Now - Order.RequestLifetime).Value!);
            await context.SaveChangesAsync();
        }

        await fixture.ApiFactory.Services.GetRequiredService<IExpiredOrderCancellationService>().CancelExpiredOrdersAsync(CancellationToken.None);

        var cancelledEmail = fixture.ApiFactory.EmailTransport.SentTo(scenario.Email)[^1];
        cancelledEmail.Subject.ShouldBe("Se canceló tu pedido");
        cancelledEmail.TextBody.ShouldContain("7 días");
    }
}
