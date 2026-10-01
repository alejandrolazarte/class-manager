namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_owner_signs_up;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_are_the_coach_of_their_instructor(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_are_the_coach_of_their_instructor_Run()
    {
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        var tokens = await anonymousClient.SignUpAsync(AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(tokens.AccessToken);

        var member = await owner.GetCurrentMemberAsync();

        var instructors = await owner.GetFromJsonAsync<List<InstructorIdentity>>(
            new Uri(ApiRoutes.Instructors, UriKind.Relative), ApiRequests.JsonOptions);
        member.InstructorId.ShouldBe(instructors!.Single().Id);
    }

    private sealed record InstructorIdentity(Guid Id);
}
