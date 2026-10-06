using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_UpdateClient_changes_the_email_used_to_sign_in;

public sealed class Then_returns_email_conflict
{
    [Fact]
    public async Task Then_returns_email_conflict_Run()
    {
        var builder = new UpdateClientUseCaseBuilder();
        builder.WithStudentAppAccount();

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClientErrorCodes.EmailUsedToSignIn);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
