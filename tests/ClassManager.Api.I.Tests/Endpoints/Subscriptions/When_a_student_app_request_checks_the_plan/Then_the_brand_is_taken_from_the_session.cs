using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_a_student_app_request_checks_the_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_is_taken_from_the_session(ApiFixture fixture)
{
    private const string BusinessesTable = "FROM [Businesses]";

    [Fact]
    public async Task Then_the_brand_is_taken_from_the_session_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterClientAsync(phoneNumber: "11 4000-1004", students: [new NewStudent("Tomás Pérez", null, null)]);
        await using var counter = new SqlCommandCounter(fixture.ConnectionString);
        using var familyApp = counter.CreateClientWithToken(
            await fixture.AcceptFamilyInvitationAsync(business.HttpClient, family.Id, StudentAppRequests.UniqueStudentEmail()));
        await familyApp.GetPushKeyAsync();

        var queries = await counter.CommandTextsAsync(() => familyApp.GetPushKeyAsync());

        queries.ShouldNotContain(query => query.Contains(BusinessesTable, StringComparison.Ordinal));
    }
}
