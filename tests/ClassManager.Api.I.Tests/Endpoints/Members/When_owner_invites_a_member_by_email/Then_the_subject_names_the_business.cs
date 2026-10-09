using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_owner_invites_a_member_by_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_subject_names_the_business(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_subject_names_the_business_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = $"{Guid.NewGuid():N}@test.test";

        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);

        fixture.ApiFactory.EmailTransport.SentTo(email).Single().Subject
            .ShouldBe($"{business.Business.BrandDisplayName} te invita a su equipo");
    }
}
