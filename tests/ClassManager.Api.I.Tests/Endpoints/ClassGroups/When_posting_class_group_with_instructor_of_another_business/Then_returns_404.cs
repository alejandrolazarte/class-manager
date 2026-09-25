namespace ClassManager.Api.I.Tests.Endpoints.ClassGroups.When_posting_class_group_with_instructor_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherInstructor = await otherBusiness.HttpClient.CreateInstructorAsync();

        using var response = await business.HttpClient.PostClassGroupAsync(ClassGroupRequests.ClassGroupFor(otherInstructor.Id));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
