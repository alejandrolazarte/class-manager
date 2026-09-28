using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.ClassGroups;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record CoachScenario(
    SeededBusiness Business,
    HttpClient Coach,
    Guid CoachInstructorId,
    Guid OtherInstructorId,
    ClassGroupResponse CoachClassGroup,
    ClassGroupResponse OtherClassGroup,
    Guid CoachStudentId,
    Guid OtherStudentId)
{
    public const string CoachFullName = "Marcos Díaz";
    public const string OtherInstructorFullName = "Laura Gómez";
    public const string CoachStudentFullName = "Tomás Pérez";
    public const string OtherStudentFullName = "Lucía Fernández";
    public const string CoachClassGroupName = "Natación Marcos";
    public const string OtherClassGroupName = "Natación Laura";

    public static readonly DateOnly ClassDate = EnrollmentRequests.Today;
}

public static class CoachScenarioRequests
{
    private const string CoachStudentPhoneNumber = "11 2233-4455";
    private const string OtherStudentPhoneNumber = "11 5566-7788";

    public static async Task<CoachScenario> SeedCoachScenarioAsync(this ApiFixture fixture)
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var coachInstructor = await owner.CreateInstructorAsync(CoachScenario.CoachFullName);
        var otherInstructor = await owner.CreateInstructorAsync(CoachScenario.OtherInstructorFullName);
        var weekdays = new[] { CoachScenario.ClassDate.DayOfWeek };
        var coachClassGroup = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(coachInstructor.Id, weekdays, "18:00") with { Name = CoachScenario.CoachClassGroupName });
        var otherClassGroup = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(otherInstructor.Id, weekdays, "19:00") with { Name = CoachScenario.OtherClassGroupName });
        var coachStudentId = await owner.RegisterStudentAsync(CoachScenario.CoachStudentFullName, CoachStudentPhoneNumber);
        var otherStudentId = await owner.RegisterStudentAsync(CoachScenario.OtherStudentFullName, OtherStudentPhoneNumber);
        await owner.EnrollAsync(coachClassGroup.Id, coachStudentId, CoachScenario.ClassDate);
        await owner.EnrollAsync(otherClassGroup.Id, otherStudentId, CoachScenario.ClassDate);
        var coach = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.Coach, coachInstructor.Id);

        return new CoachScenario(
            business,
            coach,
            coachInstructor.Id,
            otherInstructor.Id,
            coachClassGroup,
            otherClassGroup,
            coachStudentId,
            otherStudentId);
    }

    public static Task<List<StudentSummaryResponse>?> SearchStudentsAsync(this HttpClient httpClient, string search = "") =>
        httpClient.GetFromJsonAsync<List<StudentSummaryResponse>>(
            new Uri($"{ApiRoutes.Students}?search={Uri.EscapeDataString(search)}", UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<List<ClassGroupResponse>?> ListClassGroupsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<List<ClassGroupResponse>>(new Uri(ApiRoutes.ClassGroups, UriKind.Relative), ApiRequests.JsonOptions);
}
