using ClassManager.Core.Domain.Accounts;
using ClassManager.Infrastructure.Notifications;

namespace ClassManager.Api.I.Tests.Notifications.When_notification_texts_are_listed;

public sealed class Then_every_supported_language_has_them
{
    [Fact]
    public void Then_every_supported_language_has_them_Run()
    {
        NotificationTexts.All.Select(texts => texts.Language).ShouldBe(AccountLanguage.Supported, ignoreOrder: true);
    }
}
