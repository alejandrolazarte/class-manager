using Testcontainers.MsSql;

namespace ClassManager.Backoffice.Api.I.Tests.Infrastructure;

public sealed class BackofficeApiFixture : IAsyncLifetime, IDisposable
{
    private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-latest";

    private readonly MsSqlContainer _container = new MsSqlBuilder(SqlServerImage).Build();
    private BackofficeApiFactory? _apiFactory;

    public BackofficeApiFactory ApiFactory => _apiFactory ?? throw new InvalidOperationException(nameof(InitializeAsync));

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        _apiFactory = new BackofficeApiFactory(_container.GetConnectionString());
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    public void Dispose() => _apiFactory?.Dispose();
}
