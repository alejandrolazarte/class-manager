namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassPacks.When_another_business_sold_a_pack;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_listed_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var otherBusiness = await fixture.SeedStudentAppScenarioAsync();
        var otherOwner = otherBusiness.Instructors.Business.HttpClient;
        var classPack = await otherOwner.CreateAnyGroupClassPackAsync();
        await otherOwner.SellClassPackAsync(otherBusiness.ClientId, classPack.Id);

        var classPacks = await scenario.Student.GetStudentAppClassPacksAsync();

        classPacks!.Purchases.ShouldBeEmpty();
    }
}
