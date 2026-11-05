using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_the_owner_turned_off_new_order_emails;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_new_order_email_is_sent(ApiFixture fixture)
{
    [Fact]
    public async Task Then_no_new_order_email_is_sent_Run()
    {
        var ownerCommand = AuthenticationRequests.SignUpCommand();
        using var anonymous = fixture.ApiFactory.CreateClient();
        var ownerTokens = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, ownerCommand);
        using var owner = fixture.CreateClientWithToken(ownerTokens.AccessToken);
        await fixture.ChangePlanAsync((await owner.GetCurrentMemberAsync()).BusinessId, PlanCodes.Pro);
        await owner.TurnOffAsync(NotificationCategory.NewOrders, NotificationChannel.Email);
        var client = await owner.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var classPack = await owner.CreateClassPackAsync();
        var studentEmail = StudentAppRequests.UniqueStudentEmail();
        (await owner.PostStudentAppInvitationAsync(client.Id, studentEmail)).EnsureSuccessStatusCode();
        using var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(studentEmail));
        var studentTokens = (await accepted.Content.ReadFromJsonAsync<TokenResponse>(ApiRequests.JsonOptions))!;
        using var student = fixture.CreateClientWithToken(studentTokens.AccessToken);

        await student.PlaceStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));

        await fixture.WaitUntilNoTaskToAsync(ownerCommand.Email!);
        fixture.ApiFactory.EmailTransport.SentTo(ownerCommand.Email!).ShouldNotContain(email => email.Subject.StartsWith("Nuevo pedido"));
    }
}
