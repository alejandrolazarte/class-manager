using ClassManager.Core.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_an_account_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_notification_choices_are_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_notification_choices_are_deleted_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.Student.TurnOffAsync(NotificationCategory.Orders, NotificationChannel.Email);
        var businessId = scenario.Instructors.Business.Business.Id;
        await using (var before = fixture.CreateDbContext(businessId))
        {
            (await before.NotificationOptOuts.AnyAsync()).ShouldBeTrue();
        }

        await scenario.DeleteAccountAsync();

        await using var context = fixture.CreateDbContext(businessId);
        (await context.NotificationOptOuts.AnyAsync()).ShouldBeFalse();
    }
}
