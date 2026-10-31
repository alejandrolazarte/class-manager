namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_class_packs_are_sold_in_the_month;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_month_counts_them(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_month_counts_them_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 5);
        var classPack = await owner.CreateClassPackAsync();
        await owner.SellAtCounterAsync(scenario.ClientId, [OrderRequests.PackLine(classPack.Id), OrderRequests.ProductLine(product.Variants[0].Id, 1)]);

        var fees = await owner.GetMonthlyFeesAsync();

        fees!.ClassPackSales.ShouldBe(80m);
    }
}
