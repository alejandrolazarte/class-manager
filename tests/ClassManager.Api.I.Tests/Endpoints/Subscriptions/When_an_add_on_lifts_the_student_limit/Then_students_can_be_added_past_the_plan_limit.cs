using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_an_add_on_lifts_the_student_limit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_students_can_be_added_past_the_plan_limit(ApiFixture fixture)
{
    private const int FreeStudentLimit = 30;

    [Fact]
    public async Task Then_students_can_be_added_past_the_plan_limit_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        await fixture.SeedStudentsAsync(business, FreeStudentLimit);
        await fixture.AddFeatureAsync(business, Features.Students, limit: null);

        using var response = await business.HttpClient.PostClientAsync(students: [new NewStudent("Una más", null, null)]);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
