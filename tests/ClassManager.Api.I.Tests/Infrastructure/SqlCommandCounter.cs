using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed class SqlCommandCounter : IAsyncDisposable
{
    private const string CommandLogCategory = "Microsoft.EntityFrameworkCore.Database.Command";
    private const string InformationLevel = nameof(LogLevel.Information);
    private const char CommandTextStart = '\n';

    private readonly CapturingLoggerProvider _loggerProvider = new();
    private readonly WebApplicationFactory<Program> _apiFactory;

    public SqlCommandCounter(string databaseConnectionString)
    {
        var commandLogSettings = new Dictionary<string, string> { [$"Logging:LogLevel:{CommandLogCategory}"] = InformationLevel };
        _apiFactory = new BusinessApiFactory(databaseConnectionString, new FakeTimeProvider(BusinessApiFactory.Now), commandLogSettings)
            .WithWebHostBuilder(builder => builder.ConfigureServices(services => services.AddSingleton<ILoggerProvider>(_loggerProvider)));
    }

    public HttpClient CreateClientWithToken(string accessToken)
    {
        var client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiFixture.BearerScheme, accessToken);
        return client;
    }

    public async Task<int> CountAsync(Func<Task> action) => (await CommandTextsAsync(action)).Count;

    public async Task<IReadOnlyList<string>> CommandTextsAsync(Func<Task> action)
    {
        var executedBefore = ExecutedCommands().Count;
        await action();
        return [.. ExecutedCommands().Skip(executedBefore).Select(CommandText)];
    }

    public ValueTask DisposeAsync() => _apiFactory.DisposeAsync();

    private List<CapturedLogEntry> ExecutedCommands() =>
        [.. _loggerProvider.Entries.Where(entry => entry.CategoryName == CommandLogCategory && entry.LogLevel == LogLevel.Information)];

    private static string CommandText(CapturedLogEntry entry) => entry.Message[(entry.Message.IndexOf(CommandTextStart, StringComparison.Ordinal) + 1)..];
}
