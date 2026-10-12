namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_a_goal_is_set;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_measurements_include_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_measurements_include_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var studentId = await business.HttpClient.RegisterStudentAsync();
        var measurement = await business.HttpClient.CreateMeasurementAsync();

        using var response = await business.HttpClient.PutMeasurementGoalAsync(studentId, measurement.Id, 28m);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.GetStudentMeasurementsAsync(studentId))!.Single().Goal.ShouldBe(28m);
    }
}
