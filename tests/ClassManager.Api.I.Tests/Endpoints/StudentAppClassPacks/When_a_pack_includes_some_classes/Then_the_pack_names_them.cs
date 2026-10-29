using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassPacks.When_a_pack_includes_some_classes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_pack_names_them(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_pack_names_them_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var otherClass = await scenario.CreateOtherClassAsync();
        (await owner.PutBillingPlanAsync(scenario.ClientId, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        var classPack = await owner.CreateClassPackAsync("Pack 8 clases", classCount: 8, classGroupIds: [otherClass.Id]);
        await owner.SellClassPackAsync(scenario.ClientId, classPack.Id);

        var classPacks = await scenario.Student.GetStudentAppClassPacksAsync();

        classPacks!.Purchases.Single().ClassGroupNames.ShouldBe([StudentAppClassPackRequests.OtherClassName]);
    }
}
