using ClassManager.Core.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_the_feature_catalog_is_seeded;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_every_feature_in_code_has_a_row(ApiFixture fixture)
{
    [Fact]
    public async Task Then_every_feature_in_code_has_a_row_Run()
    {
        await using var context = fixture.CreateDbContext(Guid.Empty);

        var seededFeatureCodes = await context.Features.Select(feature => feature.Code).ToListAsync();

        seededFeatureCodes.Order(StringComparer.Ordinal).ShouldBe(Features.All.Order(StringComparer.Ordinal));
    }
}
