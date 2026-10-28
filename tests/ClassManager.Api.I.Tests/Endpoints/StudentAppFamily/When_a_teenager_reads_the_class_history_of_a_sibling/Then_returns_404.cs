namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_a_teenager_reads_the_class_history_of_a_sibling;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var teenager = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Lucía Pérez", StudentAppRequests.ChildBirthDate);

        using var response = await teenager.App.GetClassHistoryResponseAsync(family.Students.Single().Id, ClassHistoryRequests.CurrentMonth);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
