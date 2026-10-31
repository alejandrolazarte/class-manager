using ClassManager.Core.UseCases.Fees;
using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_payment_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_kept_as_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_kept_as_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientId = (await business.HttpClient.EnrollClientWithMonthlyPlanAsync()).ClientId;
        using var paymentResponse = await business.HttpClient.PostPaymentAsync(clientId);
        var payment = await paymentResponse.Content.ReadFromJsonAsync<PaymentResponse>(ApiRequests.JsonOptions);

        (await business.HttpClient.DeleteAsync(new Uri($"{ApiRoutes.Payments}/{payment!.Id}", UriKind.Relative))).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.Payments.AnyAsync(row => row.Id == payment.Id)).ShouldBeFalse();
        (await context.Payments
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .SingleAsync(row => row.Id == payment.Id))
            .DeletedAt.ShouldBe(BusinessApiFactory.Now);
    }
}
