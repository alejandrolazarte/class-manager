using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.UseCaseBehaviors.When_a_query_runs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_does_not_track_the_entities_it_reads(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_does_not_track_the_entities_it_reads_Run()
    {
        await using var scope = fixture.ApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        QueryTrackingBehavior? trackingDuringQuery = null;

        await UseCaseBehaviorProbe.RunAsync(scope.ServiceProvider, new ProbeQuery(), () =>
        {
            trackingDuringQuery = context.ChangeTracker.QueryTrackingBehavior;
            return UseCaseBehaviorProbe.Succeeded();
        });

        trackingDuringQuery.ShouldBe(QueryTrackingBehavior.NoTracking);
    }
}
