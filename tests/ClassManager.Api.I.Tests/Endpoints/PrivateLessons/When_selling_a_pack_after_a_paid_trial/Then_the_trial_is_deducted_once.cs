using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.PrivateLessons.When_selling_a_pack_after_a_paid_trial;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_trial_is_deducted_once(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_trial_is_deducted_once_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var pack = await business.HttpClient.CreateClassPackAsync();
        var trial = await business.HttpClient.SchedulePrivateLessonAsync(
            instructor.Id, client.Students[0].Id, EnrollmentRequests.Today, trialPrice: 25m);
        (await business.HttpClient.GetClassBalanceAsync(client.Id))!.DeductibleTrials.Single().TrialPrice.ShouldBe(25m);

        using var sale = await business.HttpClient.PostClassPackSaleAsync(client.Id, pack.Id, 55m, trial.Id);
        using var secondSale = await business.HttpClient.PostClassPackSaleAsync(client.Id, pack.Id, 55m, trial.Id);

        sale.StatusCode.ShouldBe(HttpStatusCode.Created);
        secondSale.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await business.HttpClient.GetClassBalanceAsync(client.Id))!.DeductibleTrials.ShouldBeEmpty();
    }
}
