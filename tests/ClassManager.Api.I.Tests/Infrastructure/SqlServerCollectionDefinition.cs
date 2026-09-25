namespace ClassManager.Api.I.Tests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class SqlServerCollectionDefinition : ICollectionFixture<ApiFixture>
{
    public const string Name = "SqlServer";
}
