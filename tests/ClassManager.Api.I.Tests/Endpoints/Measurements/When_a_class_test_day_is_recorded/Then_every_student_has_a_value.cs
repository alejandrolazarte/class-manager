using ClassManager.Core.UseCases.Measurements;

namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_a_class_test_day_is_recorded;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_every_student_has_a_value(ApiFixture fixture)
{
    [Fact]
    public async Task Then_every_student_has_a_value_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var owner = scenario.Business.HttpClient;
        var measurement = await owner.CreateMeasurementAsync();

        using var response = await owner.PostClassMeasurementsAsync(
            measurement.Id,
            [new ClassMeasurementEntry(scenario.InstructorStudentId, 29.7m), new ClassMeasurementEntry(scenario.OtherStudentId, 33.4m)]);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await owner.GetStudentMeasurementsAsync(scenario.OtherStudentId))!.Single().Records.Single().Value.ShouldBe(33.4m);
    }
}
