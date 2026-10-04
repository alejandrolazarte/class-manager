using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_email_delivery_is_slow;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_order_is_placed_without_waiting(ApiFixture fixture)
{
    private static readonly TimeSpan MaximumOrderDuration = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task Then_the_order_is_placed_without_waiting_Run()
    {
        var ownerCommand = AuthenticationRequests.SignUpCommand();
        using var anonymous = fixture.ApiFactory.CreateClient();
        var ownerTokens = await anonymous.SignUpAsync(ownerCommand);
        using var owner = fixture.CreateClientWithToken(ownerTokens.AccessToken);
        await fixture.ChangePlanAsync((await owner.GetCurrentMemberAsync()).BusinessId, PlanCodes.Pro);
        var client = await owner.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var classPack = await owner.CreateClassPackAsync();
        var studentEmail = StudentAppRequests.UniqueStudentEmail();
        (await owner.PostStudentAppInvitationAsync(client.Id, studentEmail)).EnsureSuccessStatusCode();
        using var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(studentEmail));
        var studentTokens = (await accepted.Content.ReadFromJsonAsync<TokenResponse>(ApiRequests.JsonOptions))!;
        using var student = fixture.CreateClientWithToken(studentTokens.AccessToken);

        Task placing;
        using (fixture.ApiFactory.EmailTransport.HoldDeliveries())
        {
            placing = student.PlaceStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));
            (await Task.WhenAny(placing, Task.Delay(MaximumOrderDuration))).ShouldBe(placing);
        }

        await placing;
        (await fixture.ApiFactory.EmailTransport.WaitForEmailToAsync(ownerCommand.Email!, email => email.Subject.StartsWith("Nuevo pedido", StringComparison.Ordinal)))
            .TextBody.ShouldContain(ClassPackRequests.PackName);
    }
}
