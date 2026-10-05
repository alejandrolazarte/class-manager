using ClassManager.Api.ErrorHandling;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_putting_phone_number_of_another_client;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var existingClient = await business.HttpClient.RegisterClientAsync();
        var clientToEdit = await business.HttpClient.RegisterClientAsync(fullName: "Luis Gómez", phoneNumber: "11 9988-7766");

        using var response = await business.HttpClient.PutClientAsync(
            clientToEdit.Id,
            new UpdateClientRequest(clientToEdit.FullName, ApiRequests.ClientPhoneNumber, null, null));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty(ResultHttpExtensions.ErrorCodeExtension).GetString().ShouldBe(ClientErrorCodes.PhoneNumberTaken);
        problem.GetProperty(ClientErrorCodes.ExistingClientIdDetail).GetGuid().ShouldBe(existingClient.Id);
    }
}
