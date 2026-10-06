using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_coach_is_invited_for_an_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_instructor_keeps_that_email(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_instructor_keeps_that_email_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Coach, instructor.Id);

        var instructors = await business.HttpClient.GetFromJsonAsync<IReadOnlyList<InstructorResponse>>(
            new Uri(ApiRoutes.Instructors, UriKind.Relative), ApiRequests.JsonOptions);

        instructors!.Single(candidate => candidate.Id == instructor.Id).Email.ShouldBe(email);
    }
}
