using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_a_student_app_request_checks_the_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_subscription_and_its_features_are_read_in_one_query(ApiFixture fixture)
{
    private const string BillingSchema = "[billing].";

    [Fact]
    public async Task Then_the_subscription_and_its_features_are_read_in_one_query_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterClientAsync(phoneNumber: "11 4000-1003", students: [new NewStudent("Tomás Pérez", null, null)]);
        await using var counter = new SqlCommandCounter(fixture.ConnectionString);
        using var familyApp = counter.CreateClientWithToken(
            await fixture.AcceptFamilyInvitationAsync(business.HttpClient, family.Id, StudentAppRequests.UniqueStudentEmail()));
        await familyApp.GetPushKeyAsync();

        var queries = await counter.CommandTextsAsync(() => familyApp.GetPushKeyAsync());

        queries.Count(query => query.Contains(BillingSchema, StringComparison.Ordinal)).ShouldBe(1);
    }
}
