using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_IsDeleted_and_DeletedOn_disagree;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_database_rejects_it(ApiFixture fixture)
{
    private const int CheckConstraintViolation = 547;

    [Fact]
    public async Task Then_the_database_rejects_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();
        (await business.HttpClient.PostPaymentAsync(clientId)).EnsureSuccessStatusCode();
        await using var context = fixture.CreateDbContext(business.Business.Id);

        var markWithoutDate = () => context.Database.ExecuteSqlAsync($"UPDATE [Payments] SET [IsDeleted] = 1 WHERE [ClientId] = {clientId}");

        (await markWithoutDate.ShouldThrowAsync<SqlException>()).Number.ShouldBe(CheckConstraintViolation);
    }
}
