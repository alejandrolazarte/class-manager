namespace ClassManager.Analyzers.Tests.Architecture.When_storage_uses_core;

public sealed class Then_ARCH008_is_reported
{
    private const string FilePath = "/src/Storage/Files/IFileStorage.cs";
    private const string Source = """
        {|ARCH008:using ClassManager.Core.Domain.Products;|}

        namespace ClassManager.Storage.Files
        {
            public interface IFileStorage { }
        }

        namespace ClassManager.Core.Domain.Products
        {
            public sealed class Product { }
        }
        """;

    [Fact]
    public Task Then_ARCH008_is_reported_Run() => AnalyzerTestRunner.RunAsync<StorageIndependenceAnalyzer>(AnalyzerTestRunner.StorageAssemblyName, FilePath, Source);
}
