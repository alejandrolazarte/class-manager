using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Instructors;
using ClassManager.Core.UseCases.StudentApp;
using ClassManager.Core.UseCases.Students;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_an_instructor_is_invited_to_the_app_as_a_contact;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_contact_uses_their_user_once_confirmed(ApiFixture fixture)
{
    private const string InstructorFullName = "Laura Gómez";

    [Fact]
    public async Task Then_the_contact_uses_their_user_once_confirmed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = StudentAppRequests.UniqueStudentEmail();
        using var instructorResponse = await business.HttpClient.PostAsJsonAsync(
            ApiRoutes.Instructors, new CreateInstructorCommand(InstructorFullName, email), ApiRequests.JsonOptions);
        var instructor = (await instructorResponse.Content.ReadFromJsonAsync<InstructorResponse>(ApiRequests.JsonOptions))!;
        using var familyResponse = await business.HttpClient.PostAsJsonAsync(
            ApiRoutes.Clients,
            new RegisterClientCommand(InstructorFullName, "11 4321-8765", null, null, [new NewStudent(InstructorFullName, new DateOnly(1990, 5, 10), null)]),
            ApiRequests.JsonOptions);
        var family = (await familyResponse.Content.ReadFromJsonAsync<ClientDetailsResponse>(ApiRequests.JsonOptions))!;
        await using var context = fixture.CreateDbContext(business.Business.Id);
        var instructorUserId = await context.Instructors.Where(row => row.Id == instructor.Id).Select(row => row.UserId).SingleAsync();
        using var question = await business.HttpClient.PostStudentAppInvitationAsync(family.Id, email);

        using var response = await business.HttpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{family.Id}{ApiRoutes.AppInvitation}",
            new InviteStudentAppRequest(email, ConfirmsContactIsAdult: true, SamePersonUserId: instructorUserId),
            ApiRequests.JsonOptions);

        var contactUserIds = await context.Clients.Where(client => client.Id == family.Id).Select(client => client.UserId)
            .Concat(context.Students.Where(student => student.ClientId == family.Id).Select(student => student.UserId))
            .ToListAsync();
        question.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        contactUserIds.ShouldBe([instructorUserId, instructorUserId]);
    }
}
