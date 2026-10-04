using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.UseCaseBehaviors.When_a_command_runs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_tracks_the_entities_it_reads(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_tracks_the_entities_it_reads_Run()
    {
        await using var scope = fixture.ApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        QueryTrackingBehavior? trackingDuringCommand = null;

        await UseCaseBehaviorProbe.RunAsync(scope.ServiceProvider, new ProbeCommand(), () =>
        {
            trackingDuringCommand = context.ChangeTracker.QueryTrackingBehavior;
            return UseCaseBehaviorProbe.Succeeded();
        });

        trackingDuringCommand.ShouldBe(QueryTrackingBehavior.TrackAll);
    }
}
