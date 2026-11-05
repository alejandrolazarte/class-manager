using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_ListMyNotificationPreferences_is_asked_by_someone_outside_the_business;

public sealed class Then_it_is_refused
{
    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        var builder = new NotificationPreferencesUseCaseBuilder();

        var response = await builder.BuildList().ExecuteAsync(new ListMyNotificationPreferencesQuery(AccountKinds.Team), CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Unauthorized);
    }
}
