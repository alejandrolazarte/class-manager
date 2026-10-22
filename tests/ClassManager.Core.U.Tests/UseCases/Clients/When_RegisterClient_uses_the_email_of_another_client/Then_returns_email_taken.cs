using ClassManager.Core.Abstractions.Persistence;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_uses_the_email_of_another_client;

public sealed class Then_returns_email_taken
{
    [Fact]
    public async Task Then_returns_email_taken_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var otherClient = User.Create("Ana Gómez", "ana@example.com", TestData.Now).Value!.WithId();
        builder.Users
            .Setup(repository => repository.FindByEmailForUpdateAsync("ana@example.com", It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherClient);
        builder.Users
            .Setup(repository => repository.GetRolesAsync(otherClient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PersonRoles(IsClient: true, IsStudent: false, IsInstructor: false, IsTeamMember: false));

        var response = await builder.Build().ExecuteAsync(RegisterClientUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(UserErrorCodes.EmailTaken);
        response.Error.FieldName.ShouldBe(nameof(User.Email));
    }
}
