using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_a_team_member_sends_app_feedback;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_feedback_inbox_gets_it_with_reply_to_them(ApiFixture fixture)
{
    private const string FeedbackInboxAddress = "classmanager.noreply@gmail.com";

    [Fact]
    public async Task Then_the_feedback_inbox_gets_it_with_reply_to_them_Run()
    {
        var message = $"Feedback {Guid.CreateVersion7()}";
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        using var anonymous = fixture.ApiFactory.CreateClient();
        var tokens = await anonymous.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
        using var member = fixture.CreateClientWithToken(tokens.AccessToken);

        using (var response = await member.PostAppFeedbackAsync(message))
        {
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        var feedback = await fixture.ApiFactory.EmailTransport.WaitForEmailToAsync(
            FeedbackInboxAddress, sentEmail => sentEmail.TextBody.Contains(message));
        feedback.ReplyTo.ShouldBe(email);
        feedback.TextBody.ShouldContain(nameof(BusinessRole.Viewer));
    }
}
