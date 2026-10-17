using ClassManager.Core.Abstractions.Persistence;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_uses_the_email_of_another_contact;

public sealed class Then_returns_email_taken
{
    [Fact]
    public async Task Then_returns_email_taken_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        builder.Users
            .Setup(repository => repository.FindKnownPersonByEmailAsync("ana@example.com", It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new KnownPerson(Guid.CreateVersion7(), "Ana María Pérez", [PersonRole.Client]));

        var response = await builder.Build().ExecuteAsync(RegisterClientUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(UserErrorCodes.EmailTaken);
        response.Error.FieldName.ShouldBe(nameof(User.Email));
    }
}
