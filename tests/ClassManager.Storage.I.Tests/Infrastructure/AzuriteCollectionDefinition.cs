namespace ClassManager.Storage.I.Tests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class AzuriteCollectionDefinition : ICollectionFixture<AzuriteFixture>
{
    public const string Name = "Azurite";
}
