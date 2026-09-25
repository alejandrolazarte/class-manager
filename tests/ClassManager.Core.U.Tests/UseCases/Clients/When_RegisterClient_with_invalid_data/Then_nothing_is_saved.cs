namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_invalid_data;

public sealed class Then_nothing_is_saved
{
    [Fact]
    public async Task Then_nothing_is_saved_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var command = RegisterClientUseCaseBuilder.ValidCommand() with { FullName = "A" };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Validation);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
