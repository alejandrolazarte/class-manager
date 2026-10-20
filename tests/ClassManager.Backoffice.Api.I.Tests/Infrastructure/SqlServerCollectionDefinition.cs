namespace ClassManager.Backoffice.Api.I.Tests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class SqlServerCollectionDefinition : ICollectionFixture<BackofficeApiFixture>
{
    public const string Name = "SqlServer";
}
