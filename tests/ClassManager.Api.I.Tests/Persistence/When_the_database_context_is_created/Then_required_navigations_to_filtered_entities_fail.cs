using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ClassManager.Api.I.Tests.Persistence.When_the_database_context_is_created;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_required_navigations_to_filtered_entities_fail(ApiFixture fixture)
{
    [Fact]
    public void Then_required_navigations_to_filtered_entities_fail_Run()
    {
        using var context = fixture.CreateDbContext(Guid.Empty);

        var warnings = context.GetService<IDbContextOptions>().FindExtension<CoreOptionsExtension>()!.WarningsConfiguration;

        warnings.GetBehavior(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning).ShouldBe(WarningBehavior.Throw);
    }
}
