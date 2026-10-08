using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_an_attending_contact_under_18;

public sealed class Then_returns_contact_must_be_adult
{
    [Fact]
    public async Task Then_returns_contact_must_be_adult_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var command = RegisterClientUseCaseBuilder.ValidCommand() with
        {
            Students = [new NewStudent(TestData.ClientFullName, TestData.Today.AddYears(-15), null)],
        };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.Code.ShouldBe(ClientErrorCodes.ContactMustBeAdult);
    }
}
