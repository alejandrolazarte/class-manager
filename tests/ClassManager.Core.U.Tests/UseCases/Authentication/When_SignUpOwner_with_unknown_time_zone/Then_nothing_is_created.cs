using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignUpOwner_with_unknown_time_zone;

public sealed class Then_nothing_is_created
{
    [Fact]
    public async Task Then_nothing_is_created_Run()
    {
        var builder = new SignUpOwnerUseCaseBuilder();
        var command = SignUpOwnerUseCaseBuilder.ValidCommand() with { TimeZoneId = "Mars/Olympus_Mons" };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        builder.Identity.Verify(
            service => service.CreateOwnerAsync(It.IsAny<OwnerAccount>(), It.IsAny<CancellationToken>()),
            Times.Never);
        response.Error!.FieldName.ShouldBe(nameof(SignUpOwnerCommand.TimeZoneId));
    }
}
