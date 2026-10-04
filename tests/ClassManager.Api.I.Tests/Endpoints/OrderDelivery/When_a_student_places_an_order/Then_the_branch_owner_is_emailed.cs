using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_a_student_places_an_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_branch_owner_is_emailed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_branch_owner_is_emailed_Run()
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

        await student.PlaceStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));

        var placedEmail = fixture.ApiFactory.EmailTransport.SentTo(ownerCommand.Email!)[^1];
        placedEmail.Subject.ShouldBe($"Nuevo pedido n.º 1 de {ApiRequests.ClientFullName}");
        placedEmail.TextBody.ShouldContain(ClassPackRequests.PackName);
    }
}
