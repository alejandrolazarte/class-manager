using System.Globalization;
using ClassManager.Core.UseCases.ClassPacks;
using ClassManager.Core.UseCases.Families;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record PackFamilyScenario(FamilyScenario Scenario, ClassPackResponse ClassPack)
{
    public CoachScenario Coaches => Scenario.Coaches;
    public HttpClient Owner => Scenario.Coaches.Business.HttpClient;
    public HttpClient Family => Scenario.Family;
    public Guid StudentId => Scenario.Coaches.CoachStudentId;
    public Guid PackClassGroupId => Scenario.Coaches.OtherClassGroup.Id;
}

public static class PackBookingRequests
{
    private const string IsoDateFormat = "yyyy-MM-dd";
    private const string PackName = "Pack natación";

    public static string PackClassesPath(Guid studentId) =>
        $"{ApiRoutes.Family}{ApiRoutes.FamilyStudents}/{studentId}{ApiRoutes.PackClasses}";

    public static string PackClassPath(Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        $"{PackClassesPath(studentId)}/{classGroupId}/{sessionDate.ToString(IsoDateFormat, CultureInfo.InvariantCulture)}";

    public static async Task<PackFamilyScenario> SeedPackFamilyScenarioAsync(this ApiFixture fixture, int classCount = 4)
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var classPack = await owner.CreateClassPackAsync(PackName, classCount, [scenario.Coaches.OtherClassGroup.Id]);
        await owner.SellClassPackAsync(scenario.FamilyId, classPack.Id);
        return new PackFamilyScenario(scenario, classPack);
    }

    public static Task<FamilyPackClassesResponse?> GetPackClassesAsync(this HttpClient httpClient, Guid studentId) =>
        httpClient.GetFromJsonAsync<FamilyPackClassesResponse>(new Uri(PackClassesPath(studentId), UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutPackClassAsync(this HttpClient httpClient, Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        httpClient.PutAsJsonAsync(PackClassPath(studentId, classGroupId, sessionDate), new { }, ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeletePackClassAsync(this HttpClient httpClient, Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        httpClient.DeleteAsync(new Uri(PackClassPath(studentId, classGroupId, sessionDate), UriKind.Relative));

    public static async Task BookPackClassAsync(this PackFamilyScenario scenario, DateOnly? sessionDate = null)
    {
        using var response = await scenario.Family.PutPackClassAsync(scenario.StudentId, scenario.PackClassGroupId, sessionDate ?? CoachScenario.ClassDate);
        response.EnsureSuccessStatusCode();
    }
}
