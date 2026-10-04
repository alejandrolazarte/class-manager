using ClassManager.Api.Orders;
using ClassManager.Core.Domain.Orders;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_an_unpaid_order_expires;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_is_emailed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_is_emailed_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var classPack = await scenario.Coaches.Business.HttpClient.CreateClassPackAsync();
        await using (var context = fixture.CreateDbContext(scenario.Coaches.Business.Business.Id))
        {
            var pack = await context.ClassPacks.FindAsync(classPack.Id);
            context.Orders.Add(Order.Request(
                scenario.ClientId, [OrderLine.ForClassPack(pack!, null).Value!], null, BusinessApiFactory.Now - Order.RequestLifetime).Value!);
            await context.SaveChangesAsync();
        }

        await fixture.ApiFactory.Services.GetRequiredService<IExpiredOrderCancellationService>().CancelExpiredOrdersAsync(CancellationToken.None);

        var cancelledEmail = await fixture.ApiFactory.EmailTransport.WaitForEmailToAsync(scenario.Email, email => email.Subject == "Se canceló tu pedido");
        cancelledEmail.TextBody.ShouldContain("7 días");
    }
}
