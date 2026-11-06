using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_GetEmailUnsubscription_reads_a_valid_link;

public sealed class Then_nothing_is_turned_off
{
    [Fact]
    public async Task Then_nothing_is_turned_off_Run()
    {
        var builder = new EmailUnsubscribeUseCaseBuilder();

        var response = await builder.BuildGet().ExecuteAsync(
            new GetEmailUnsubscriptionQuery(EmailUnsubscribeUseCaseBuilder.ValidToken), CancellationToken.None);

        response.Value!.ShouldBe(new EmailUnsubscriptionResponse(TestData.Business().BrandDisplayName, NotificationCategory.Orders, IsUnsubscribed: false));
        builder.OptOuts.OptOuts.ShouldBeEmpty();
    }
}
