namespace ClassManager.Api.I.Tests.Endpoints.ClassGroups.When_posting_class_group_without_weekdays;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400_with_field_error(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_with_field_error_Run()
    {
        const string WeekdaysField = "weekdays";
        const string ErrorsProperty = "errors";
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();

        using var response = await business.HttpClient.PostClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id, weekdays: []));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty(ErrorsProperty).TryGetProperty(WeekdaysField, out _).ShouldBeTrue();
    }
}
