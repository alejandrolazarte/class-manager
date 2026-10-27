namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_a_student_who_paid_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_month_still_counts_the_payment(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_month_still_counts_the_payment_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        await owner.SetDefaultFeeAsync();
        var clientId = await owner.EnrollClientAsync();
        (await owner.PostPaymentAsync(clientId)).EnsureSuccessStatusCode();
        var studentId = (await owner.GetClientAsync(clientId)).Students.Single().Id;

        (await owner.DeleteStudentAsync(studentId)).EnsureSuccessStatusCode();

        (await owner.GetMonthlyFeesAsync())!.TotalPaid.ShouldBe(FeeRequests.DefaultFee);
    }
}
