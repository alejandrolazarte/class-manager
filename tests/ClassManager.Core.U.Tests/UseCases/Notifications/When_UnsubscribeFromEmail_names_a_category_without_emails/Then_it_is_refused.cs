using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_UnsubscribeFromEmail_names_a_category_without_emails;

public sealed class Then_it_is_refused
{
    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        var builder = new EmailUnsubscribeUseCaseBuilder().ForCategory(NotificationCategory.News);

        var response = await builder.BuildUnsubscribe().ExecuteAsync(
            new UnsubscribeFromEmailCommand(EmailUnsubscribeUseCaseBuilder.ValidToken), CancellationToken.None);

        response.Error!.Code.ShouldBe(NotificationErrorCodes.InvalidUnsubscribeLink);
    }
}
