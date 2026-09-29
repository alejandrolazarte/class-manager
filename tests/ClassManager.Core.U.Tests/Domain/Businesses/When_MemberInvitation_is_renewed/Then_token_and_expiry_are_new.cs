using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_MemberInvitation_is_renewed;

public sealed class Then_token_and_expiry_are_new
{
    [Fact]
    public void Then_token_and_expiry_are_new_Run()
    {
        var invitation = MemberInvitation.Create(
            "lucia@example.com", MemberRole.System(BusinessRole.Viewer), null, new string('a', 64), Guid.CreateVersion7(), TestData.Now).Value!;
        var renewedAt = TestData.Now.AddDays(3);

        invitation.Renew(new string('b', 64), renewedAt);

        invitation.TokenHash.ShouldBe(new string('b', 64));
        invitation.ExpiresAt.ShouldBe(renewedAt + MemberInvitation.Lifetime);
    }
}
