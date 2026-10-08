using ClassManager.Core.Domain.Makeups;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMakeups.When_the_school_cancels_a_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_students_get_a_makeup_credit(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_students_get_a_makeup_credit_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var instructors = scenario.Instructors;
        (await instructors.Business.HttpClient.PutSessionCancellationAsync(instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate, "Feriado"))
            .EnsureSuccessStatusCode();

        var makeups = await scenario.Student.GetMakeupsAsync(instructors.InstructorStudentId);

        makeups!.Credits.ShouldHaveSingleItem().Reason.ShouldBe(MakeupReason.Cancelled);
    }
}
