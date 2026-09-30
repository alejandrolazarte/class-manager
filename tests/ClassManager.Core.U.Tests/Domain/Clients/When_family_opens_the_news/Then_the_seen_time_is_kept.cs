using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_family_opens_the_news;

public sealed class Then_the_seen_time_is_kept
{
    [Fact]
    public void Then_the_seen_time_is_kept_Run()
    {
        var account = ClientAccount.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), TestData.Now);

        account.MarkNewsSeen(TestData.Now.AddDays(1));

        account.NewsSeenAt.ShouldBe(TestData.Now.AddDays(1));
    }
}
