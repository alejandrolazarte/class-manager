using ClassManager.Core.Domain.Users;

namespace ClassManager.Api.I.Tests.Persistence.When_a_column_says_who_did_something;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_points_to_a_user(ApiFixture fixture)
{
    private const string WhoDidSomethingSuffix = "ByUserId";

    [Fact]
    public void Then_it_points_to_a_user_Run()
    {
        using var context = fixture.CreateDbContext(0L);

        var columnsWithoutUser = context.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties()
                .Where(property => property.Name.EndsWith(WhoDidSomethingSuffix, StringComparison.Ordinal))
                .Where(property => !property.GetContainingForeignKeys().Any(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(User)))
                .Select(property => $"{entityType.ClrType.Name}.{property.Name}"))
            .ToList();

        columnsWithoutUser.ShouldBeEmpty();
    }
}
