using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Students;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_an_add_on_lowers_the_student_limit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_adding_past_it_is_refused(ApiFixture fixture)
{
    private const int AgreedStudentLimit = 2;

    [Fact]
    public async Task Then_adding_past_it_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Enterprise);
        await fixture.AddFeatureAsync(business, Features.Students, AgreedStudentLimit);
        await fixture.SeedStudentsAsync(business, AgreedStudentLimit);

        using var response = await business.HttpClient.PostClientAsync(students: [new NewStudent("Una más", null, null)]);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(FeatureErrorCodes.LimitReached);
    }
}
