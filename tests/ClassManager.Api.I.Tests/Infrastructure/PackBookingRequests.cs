using System.Globalization;
using ClassManager.Core.UseCases.ClassPacks;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record PackStudentAppScenario(StudentAppScenario Scenario, ClassPackResponse ClassPack)
{
    public InstructorScenario Instructors => Scenario.Instructors;
    public HttpClient Owner => Scenario.Instructors.Business.HttpClient;
    public HttpClient Student => Scenario.Student;
    public Guid StudentId => Scenario.Instructors.InstructorStudentId;
    public Guid PackClassGroupId => Scenario.Instructors.OtherClassGroup.Id;
}

public static class PackBookingRequests
{
    private const string IsoDateFormat = "yyyy-MM-dd";
    private const string PackName = "Pack natación";

    public static string PackClassesPath(Guid studentId) =>
        $"{ApiRoutes.StudentApp}{ApiRoutes.AccountStudents}/{studentId}{ApiRoutes.PackClasses}";

    public static string PackClassPath(Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        $"{PackClassesPath(studentId)}/{classGroupId}/{sessionDate.ToString(IsoDateFormat, CultureInfo.InvariantCulture)}";

    public static async Task<PackStudentAppScenario> SeedPackStudentAppScenarioAsync(this ApiFixture fixture, int classCount = 4)
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var classPack = await owner.CreateClassPackAsync(PackName, classCount, [scenario.Instructors.OtherClassGroup.Id]);
        await owner.SellClassPackAsync(scenario.ClientId, classPack.Id);
        return new PackStudentAppScenario(scenario, classPack);
    }

    public static Task<StudentAppPackClassesResponse?> GetPackClassesAsync(this HttpClient httpClient, Guid studentId) =>
        httpClient.GetFromJsonAsync<StudentAppPackClassesResponse>(new Uri(PackClassesPath(studentId), UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutPackClassAsync(this HttpClient httpClient, Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        httpClient.PutAsJsonAsync(PackClassPath(studentId, classGroupId, sessionDate), new { }, ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeletePackClassAsync(this HttpClient httpClient, Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        httpClient.DeleteAsync(new Uri(PackClassPath(studentId, classGroupId, sessionDate), UriKind.Relative));

    public static async Task BookPackClassAsync(this PackStudentAppScenario scenario, DateOnly? sessionDate = null)
    {
        using var response = await scenario.Student.PutPackClassAsync(scenario.StudentId, scenario.PackClassGroupId, sessionDate ?? InstructorScenario.ClassDate);
        response.EnsureSuccessStatusCode();
    }
}
