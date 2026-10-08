using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.ClassGroups;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record InstructorScenario(
    SeededBusiness Business,
    HttpClient Instructor,
    Guid InstructorId,
    Guid OtherInstructorId,
    ClassGroupResponse InstructorClassGroup,
    ClassGroupResponse OtherClassGroup,
    Guid InstructorStudentId,
    Guid OtherStudentId)
{
    public const string InstructorFullName = "Marcos Díaz";
    public const string OtherInstructorFullName = "Laura Gómez";
    public const string InstructorStudentFullName = "Tomás Pérez";
    public const string OtherStudentFullName = "Lucía Fernández";
    public const string InstructorClassGroupName = "Natación Marcos";
    public const string OtherClassGroupName = "Natación Laura";

    public static readonly DateOnly ClassDate = EnrollmentRequests.Today;
}

public static class InstructorScenarioRequests
{
    private const string InstructorStudentPhoneNumber = "11 2233-4455";
    private const string OtherStudentPhoneNumber = "11 5566-7788";

    public static async Task<InstructorScenario> SeedInstructorScenarioAsync(this ApiFixture fixture)
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var linkedInstructor = await owner.CreateInstructorAsync(InstructorScenario.InstructorFullName);
        var otherInstructor = await owner.CreateInstructorAsync(InstructorScenario.OtherInstructorFullName);
        var weekdays = new[] { InstructorScenario.ClassDate.DayOfWeek };
        var instructorClassGroup = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(linkedInstructor.Id, weekdays, "18:00") with { Name = InstructorScenario.InstructorClassGroupName });
        var otherClassGroup = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(otherInstructor.Id, weekdays, "19:00") with { Name = InstructorScenario.OtherClassGroupName });
        var instructorStudentId = await owner.RegisterStudentAsync(InstructorScenario.InstructorStudentFullName, InstructorStudentPhoneNumber);
        var otherStudentId = await owner.RegisterStudentAsync(InstructorScenario.OtherStudentFullName, OtherStudentPhoneNumber);
        await owner.EnrollAsync(instructorClassGroup.Id, instructorStudentId, InstructorScenario.ClassDate);
        await owner.EnrollAsync(otherClassGroup.Id, otherStudentId, InstructorScenario.ClassDate);
        var instructor = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.Instructor, linkedInstructor.Id);

        return new InstructorScenario(
            business,
            instructor,
            linkedInstructor.Id,
            otherInstructor.Id,
            instructorClassGroup,
            otherClassGroup,
            instructorStudentId,
            otherStudentId);
    }

    public static Task<List<StudentSummaryResponse>?> SearchStudentsAsync(this HttpClient httpClient, string search = "") =>
        httpClient.GetFromJsonAsync<List<StudentSummaryResponse>>(
            new Uri($"{ApiRoutes.Students}?search={Uri.EscapeDataString(search)}", UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<List<ClassGroupResponse>?> ListClassGroupsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<List<ClassGroupResponse>>(new Uri(ApiRoutes.ClassGroups, UriKind.Relative), ApiRequests.JsonOptions);
}
