namespace ClassManager.Storage.I.Tests.Blobs.When_the_connection_string_is_missing;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_resolving_the_storage_fails(AzuriteFixture fixture)
{
    [Fact]
    public void Then_resolving_the_storage_fails_Run()
    {
        Should.Throw<InvalidOperationException>(() => fixture.CreateStorage(connectionString: string.Empty));
    }
}
