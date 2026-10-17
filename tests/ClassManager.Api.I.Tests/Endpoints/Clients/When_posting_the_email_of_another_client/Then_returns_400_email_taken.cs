using ClassManager.Api.ErrorHandling;
using ClassManager.Core.Domain.Users;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_posting_the_email_of_another_client;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400_email_taken(ApiFixture fixture)
{
    private const string SharedEmail = "ana@example.com";

    [Fact]
    public async Task Then_returns_400_email_taken_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var firstClient = await business.HttpClient.PostAsJsonAsync(
            ApiRoutes.Clients,
            new RegisterClientCommand(ApiRequests.ClientFullName, ApiRequests.ClientPhoneNumber, SharedEmail, null, null),
            ApiRequests.JsonOptions);

        using var response = await business.HttpClient.PostAsJsonAsync(
            ApiRoutes.Clients,
            new RegisterClientCommand("Sofía Díaz", "11 4321-8765", SharedEmail.ToUpperInvariant(), null, null),
            ApiRequests.JsonOptions);

        var problem = await response.ReadProblemAsync();
        firstClient.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problem.GetProperty(ResultHttpExtensions.ErrorCodeExtension).GetString().ShouldBe(UserErrorCodes.EmailTaken);
    }
}
