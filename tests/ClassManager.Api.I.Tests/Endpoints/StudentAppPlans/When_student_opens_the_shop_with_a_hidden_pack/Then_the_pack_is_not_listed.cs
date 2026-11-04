using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPlans.When_student_opens_the_shop_with_a_hidden_pack;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_pack_is_not_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_pack_is_not_listed_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        await owner.CreateClassPackAsync();
        using (var hidden = await owner.PostAsJsonAsync(
            ApiRoutes.ClassPacks,
            new CreateClassPackCommand("Pack del staff", 4, 80m, 1, ClassPackRequests.PrivateLessonMinutes, IsVisibleInApp: false),
            ApiRequests.JsonOptions))
        {
            hidden.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var shop = await scenario.Student.GetStudentAppShopAsync();

        shop.Packs.Select(pack => pack.Name).ShouldBe([ClassPackRequests.PackName]);
    }
}
