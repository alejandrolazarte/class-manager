namespace ClassManager.Analyzers.Tests.Architecture.When_storage_azureblob_uses_tenancy;

public sealed class Then_ARCH008_is_reported
{
    private const string FilePath = "/src/Storage.AzureBlob/Blobs/AzureBlobFileStorage.cs";
    private const string Source = """
        {|ARCH008:using ClassManager.Tenancy;|}

        namespace ClassManager.Storage.AzureBlob.Blobs
        {
            public sealed class AzureBlobFileStorage { }
        }

        namespace ClassManager.Tenancy
        {
            public interface ITenantContext { }
        }
        """;

    [Fact]
    public Task Then_ARCH008_is_reported_Run() => AnalyzerTestRunner.RunAsync<StorageIndependenceAnalyzer>(AnalyzerTestRunner.StorageAzureBlobAssemblyName, FilePath, Source);
}
