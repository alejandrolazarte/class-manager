using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Api.I.Tests.Endpoints.ClassGroups.When_reading_the_week_of_an_instructor_with_a_group;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_group_is_listed_on_each_weekday(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_group_is_listed_on_each_weekday_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();

        using var response = await business.HttpClient.GetInstructorWeekAsync(classGroup.InstructorId);

        var slots = await response.Content.ReadFromJsonAsync<List<InstructorWeekSlotResponse>>(ApiRequests.JsonOptions);
        slots!.Select(slot => (slot.Weekday, slot.StartTime, slot.ClassGroupName)).ShouldBe(
            classGroup.Weekdays.Select(weekday => (weekday, classGroup.StartTime, (string?)classGroup.Name)));
    }
}
