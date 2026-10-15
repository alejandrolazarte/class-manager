using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Emails;

namespace ClassManager.Core.U.Tests.Emails.When_email_texts_are_listed;

public sealed class Then_every_supported_language_has_them
{
    [Fact]
    public void Then_every_supported_language_has_them_Run()
    {
        EmailTexts.All.Select(texts => texts.Language).ShouldBe(AccountLanguage.Supported, ignoreOrder: true);
    }
}
