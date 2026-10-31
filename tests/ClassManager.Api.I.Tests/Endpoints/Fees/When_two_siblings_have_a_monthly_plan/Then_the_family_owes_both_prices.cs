using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_two_siblings_have_a_monthly_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_owes_both_prices(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_family_owes_both_prices_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var plan = await business.HttpClient.CreateMonthlyPlanAsync(price: 20000m);
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent("Tomás Pérez", null, null), new NewStudent("Lucía Pérez", null, null)]);
        await business.HttpClient.EnrollAsync(classGroup.Id, client.Students[0].Id);

        foreach (var student in client.Students)
        {
            (await business.HttpClient.PutStudentPlanAsync(student.Id, plan.Id)).EnsureSuccessStatusCode();
        }

        (await business.HttpClient.GetMonthlyFeesAsync())!.Clients.Single().Fee.ShouldBe(40000m);
    }
}
