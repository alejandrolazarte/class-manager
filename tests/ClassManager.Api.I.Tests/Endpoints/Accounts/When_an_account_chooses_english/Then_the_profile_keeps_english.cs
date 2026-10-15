using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_an_account_chooses_english;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_profile_keeps_english(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_profile_keeps_english_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        (await scenario.Student.GetMyAccountAsync())!.PreferredLanguage.ShouldBe(AccountLanguage.Spanish);
        using (var response = await scenario.Student.PutMyLanguageAsync(AccountLanguage.English))
        {
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        var account = await scenario.Student.GetMyAccountAsync();

        account!.PreferredLanguage.ShouldBe(AccountLanguage.English);
    }
}
