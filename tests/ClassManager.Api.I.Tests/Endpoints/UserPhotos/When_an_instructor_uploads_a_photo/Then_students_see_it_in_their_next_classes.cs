namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_an_instructor_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_students_see_it_in_their_next_classes(ApiFixture fixture)
{
    [Fact]
    public async Task Then_students_see_it_in_their_next_classes_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        await scenario.Instructors.Instructor.ChangeMyPhotoAsync();

        var home = await scenario.Student.GetStudentAppHomeAsync();
        var nextClass = home!.Students.SelectMany(student => student.NextClasses)
            .First(nextClass => nextClass.ClassGroupId == scenario.Instructors.InstructorClassGroup.Id);
        nextClass.InstructorPhotoUrl.ShouldNotBeNull();
    }
}
