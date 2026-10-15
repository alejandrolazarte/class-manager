using ClassManager.Core.Emails;

namespace ClassManager.Core.U.Tests.Emails.When_the_language_is_not_supported;

public sealed class Then_spanish_texts_are_used
{
    [Fact]
    public void Then_spanish_texts_are_used_Run()
    {
        EmailTexts.For("fr").ShouldBeSameAs(EmailTexts.Spanish);
    }
}
