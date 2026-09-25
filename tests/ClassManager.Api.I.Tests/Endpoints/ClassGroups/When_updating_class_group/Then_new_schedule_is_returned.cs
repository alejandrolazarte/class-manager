using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Api.I.Tests.Endpoints.ClassGroups.When_updating_class_group;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_new_schedule_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_new_schedule_is_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var classGroup = await business.HttpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));

        using var response = await business.HttpClient.PutAsJsonAsync(
            $"{ApiRoutes.ClassGroups}/{classGroup.Id}",
            ClassGroupRequests.ClassGroupFor(instructor.Id, weekdays: [DayOfWeek.Monday], startTime: "09:00", durationMinutes: 60),
            ApiRequests.JsonOptions);

        var updatedClassGroup = await response.Content.ReadFromJsonAsync<ClassGroupResponse>(ApiRequests.JsonOptions);
        updatedClassGroup!.EndTime.ShouldBe("10:00");
    }
}
