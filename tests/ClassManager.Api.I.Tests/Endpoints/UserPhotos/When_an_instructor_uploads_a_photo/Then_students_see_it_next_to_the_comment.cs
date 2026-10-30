namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_an_instructor_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_students_see_it_next_to_the_comment(ApiFixture fixture)
{
    private const string Comment = "Muy buen ritmo";

    [Fact]
    public async Task Then_students_see_it_next_to_the_comment_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var instructors = scenario.Instructors;
        (await instructors.Instructor.PutFeedbackAsync(
            instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate, instructors.InstructorStudentId, Comment))
            .EnsureSuccessStatusCode();

        await instructors.Instructor.ChangeMyPhotoAsync();

        var home = await scenario.Student.GetStudentAppHomeAsync();
        home!.Students.Single().LatestFeedback!.InstructorPhotoUrl.ShouldNotBeNull();
    }
}
