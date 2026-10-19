using ClassManager.Core.UseCases.Fees;
using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_user_soft_deletes_a_row;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_records_who_deleted_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_records_who_deleted_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();
        using var paymentResponse = await business.HttpClient.PostPaymentAsync(clientId);
        var payment = await paymentResponse.Content.ReadFromJsonAsync<PaymentResponse>(ApiRequests.JsonOptions);

        (await business.HttpClient.DeleteAsync(new Uri($"{ApiRoutes.Payments}/{payment!.Id}", UriKind.Relative))).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        var ownerUserId = await context.Users.Where(user => user.AccountId == business.OwnerAccountId).Select(user => user.Id).SingleAsync();
        (await context.Payments
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .SingleAsync(row => row.Id == payment.Id))
            .DeletedByUserId.ShouldBe(ownerUserId);
    }
}
