using ClassManager.Core.Abstractions.Persistence;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_uses_the_email_of_another_contact;

public sealed class Then_returns_email_taken
{
    [Fact]
    public async Task Then_returns_email_taken_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var otherPerson = User.Create("Ana María Pérez", "ana@example.com", TestData.Now).Value!;
        builder.Users.HasPersonWithTheirEmail(otherPerson, new PersonRoles(true, false, false, false));

        var response = await builder.Build().ExecuteAsync(RegisterClientUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(UserErrorCodes.EmailTaken);
        response.Error.FieldName.ShouldBe(nameof(User.Email));
    }
}
