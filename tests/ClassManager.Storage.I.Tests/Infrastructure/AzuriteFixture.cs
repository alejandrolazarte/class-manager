using ClassManager.Storage.AzureBlob.Blobs;
using ClassManager.Storage.AzureBlob.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Azurite;

namespace ClassManager.Storage.I.Tests.Infrastructure;

public sealed class AzuriteFixture : IAsyncLifetime
{
    public const string AzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.37.0";

    public const string SkipApiVersionCheckFlag = "--skipApiVersionCheck";

    private const string ConnectionStringsSection = "ConnectionStrings";

    private readonly AzuriteContainer _container = new AzuriteBuilder(AzuriteImage)
        .WithInMemoryPersistence()
        .WithCommand(SkipApiVersionCheckFlag)
        .Build();

    public HttpClient AnonymousClient { get; } = new();

    public Task InitializeAsync() => _container.StartAsync();

    public async Task DisposeAsync()
    {
        AnonymousClient.Dispose();
        await _container.DisposeAsync();
    }

    public IFileStorage CreateStorage(string? connectionString = null)
    {
        var settings = new Dictionary<string, string?>
        {
            [$"{ConnectionStringsSection}:{BlobStorageOptions.ConnectionStringName}"] = connectionString ?? _container.GetConnectionString(),
            [$"{BlobStorageOptions.SectionName}:{nameof(BlobStorageOptions.ContainerName)}"] = $"files-{Guid.NewGuid():N}",
        };
        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(settings).Build())
            .AddAzureBlobFileStorage();

        return services.BuildServiceProvider().GetRequiredService<IFileStorage>();
    }
}
