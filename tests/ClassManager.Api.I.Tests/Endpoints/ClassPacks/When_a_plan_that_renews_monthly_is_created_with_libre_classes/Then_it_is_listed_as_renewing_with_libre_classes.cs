using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_a_plan_that_renews_monthly_is_created_with_libre_classes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_listed_as_renewing_with_libre_classes(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_listed_as_renewing_with_libre_classes_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        await business.HttpClient.CreateMonthlyPlanAsync();

        var catalog = await business.HttpClient.GetFromJsonAsync<IReadOnlyList<ClassPackResponse>>(ApiRoutes.ClassPacks, ApiRequests.JsonOptions);
        var plan = catalog!.Single();
        plan.RenewsMonthly.ShouldBeTrue();
        plan.ClassCount.ShouldBeNull();
        plan.AppliesFamilyDiscount.ShouldBeTrue();
    }
}
