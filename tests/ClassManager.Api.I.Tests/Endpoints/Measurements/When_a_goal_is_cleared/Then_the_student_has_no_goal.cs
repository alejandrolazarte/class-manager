namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_a_goal_is_cleared;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_has_no_goal(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_has_no_goal_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var studentId = await business.HttpClient.RegisterStudentAsync();
        var measurement = await business.HttpClient.CreateMeasurementAsync();
        (await business.HttpClient.PutMeasurementGoalAsync(studentId, measurement.Id, 28m)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.PutMeasurementGoalAsync(studentId, measurement.Id, null);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.GetStudentMeasurementsAsync(studentId))!.Single().Goal.ShouldBeNull();
    }
}
