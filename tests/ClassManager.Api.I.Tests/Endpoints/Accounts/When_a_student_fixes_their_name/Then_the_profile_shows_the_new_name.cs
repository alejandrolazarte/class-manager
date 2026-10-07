namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_a_student_fixes_their_name;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_profile_shows_the_new_name(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_profile_shows_the_new_name_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        using (var response = await scenario.Student.PutMyFullNameAsync("  Ana María Pérez "))
        {
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        var account = await scenario.Student.GetMyAccountAsync();

        account!.FullName.ShouldBe("Ana María Pérez");
    }
}
