using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Passkeys;

namespace ClassManager.Core.U.Tests.UseCases.Passkeys.When_RemoveMyPasskey_does_not_find_it;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.AccountId).Returns(Guid.CreateVersion7());
        var useCase = new RemoveMyPasskeyUseCase(currentUser.Object, new Mock<IPasskeyIdentityService>().Object);

        var response = await useCase.ExecuteAsync(new RemoveMyPasskeyCommand("unknown"), CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.PasskeyNotFound);
    }
}
