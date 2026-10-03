using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Students;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_brand_has_as_many_students_as_its_plan_allows;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_adding_another_is_refused(ApiFixture fixture)
{
    private const int FreeStudentLimit = 30;

    [Fact]
    public async Task Then_adding_another_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        await fixture.SeedStudentsAsync(business, FreeStudentLimit);

        using var response = await business.HttpClient.PostClientAsync(students: [new NewStudent("Una más", null, null)]);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(FeatureErrorCodes.LimitReached);
        problem.GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.Students);
    }
}
