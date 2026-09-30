using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.U.Tests.Domain.Notifications.When_MemberPushSubscription_endpoint_is_not_https;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var result = MemberPushSubscription.Create(Guid.CreateVersion7(), "http://push.example.com/1", "key", "auth", TestData.Now);

        result.Error!.Code.ShouldBe(PushErrorCodes.InvalidEndpoint);
    }
}
