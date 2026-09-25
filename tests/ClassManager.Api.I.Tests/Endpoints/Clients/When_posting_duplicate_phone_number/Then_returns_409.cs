using ClassManager.Api.ErrorHandling;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_posting_duplicate_phone_number;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var existingClient = await business.HttpClient.RegisterClientAsync();

        using var response = await business.HttpClient.PostClientAsync(fullName: "Another Ana", phoneNumber: ApiRequests.NormalizedClientPhoneNumber);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty(ResultHttpExtensions.ErrorCodeExtension).GetString().ShouldBe(ClientErrorCodes.PhoneNumberTaken);
        problem.GetProperty(ClientErrorCodes.ExistingClientIdDetail).GetGuid().ShouldBe(existingClient.Id);
    }
}
