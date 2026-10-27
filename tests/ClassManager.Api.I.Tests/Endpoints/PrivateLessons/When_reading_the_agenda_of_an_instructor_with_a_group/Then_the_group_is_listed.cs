using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Api.I.Tests.Endpoints.PrivateLessons.When_reading_the_agenda_of_an_instructor_with_a_group;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_group_is_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_group_is_listed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();

        using var response = await business.HttpClient.GetInstructorAgendaAsync(classGroup.InstructorId, EnrollmentRequests.Today);

        var slots = await response.Content.ReadFromJsonAsync<List<InstructorAgendaSlotResponse>>(ApiRequests.JsonOptions);
        slots.ShouldHaveSingleItem().ShouldBe(new InstructorAgendaSlotResponse(
            classGroup.StartTime, classGroup.DurationMinutes, InstructorAgendaSlotKind.ClassGroup, classGroup.Name));
    }
}
