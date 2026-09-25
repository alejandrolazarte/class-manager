using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Api.I.Tests.Endpoints.ClassGroups.When_posting_valid_class_group;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_201_with_end_time(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_201_with_end_time_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();

        using var response = await business.HttpClient.PostClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var classGroup = await response.Content.ReadFromJsonAsync<ClassGroupResponse>(ApiRequests.JsonOptions);
        classGroup!.EndTime.ShouldBe("18:45");
        classGroup.Weekdays.ShouldBe([DayOfWeek.Tuesday, DayOfWeek.Thursday]);
        response.Headers.Location!.ToString().ShouldBe($"{ApiRoutes.ClassGroups}/{classGroup.Id}");
    }
}
