using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_a_minor_signs_up_a_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand() with
        {
            OwnerBirthDate = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddYears(-17),
        };

        using var response = await client.PostSignUpAsync(command);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AuthenticationErrorCodes.OwnerMustBeAdult);
    }
}
