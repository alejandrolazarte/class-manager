using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_RequestSignUp_with_unknown_time_zone;

public sealed class Then_nothing_is_saved
{
    [Fact]
    public async Task Then_nothing_is_saved_Run()
    {
        var builder = new RequestSignUpUseCaseBuilder();
        var command = RequestSignUpUseCaseBuilder.ValidCommand() with { TimeZoneId = "Mars/Olympus_Mons" };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(RequestSignUpCommand.TimeZoneId));
        builder.PendingSignUps.Verify(repository => repository.Add(It.IsAny<PendingSignUp>()), Times.Never);
        builder.EmailSender.Verify(sender => sender.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
