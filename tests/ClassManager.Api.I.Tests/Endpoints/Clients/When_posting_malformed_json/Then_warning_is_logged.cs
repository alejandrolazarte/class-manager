using System.Net.Http.Headers;
using System.Text;
using ClassManager.Api.ErrorHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_posting_malformed_json;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_warning_is_logged(ApiFixture fixture)
{
    private static readonly string ExceptionHandlerCategory = typeof(GlobalExceptionHandler).FullName!;

    [Fact]
    public async Task Then_warning_is_logged_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var loggerProvider = new CapturingLoggerProvider();
        await using var apiFactory = new BusinessApiFactory(fixture.ConnectionString).WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => services.AddSingleton<ILoggerProvider>(loggerProvider)));
        using var httpClient = apiFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(ApiFixture.BearerScheme, fixture.CreateAccessToken(business.Business.Id));

        using var response = await httpClient.PostRawClientBodyAsync(Encoding.UTF8.GetBytes("{\"fullName\": \"Ana\""));

        loggerProvider.Entries
            .Where(entry => entry.CategoryName == ExceptionHandlerCategory)
            .Select(entry => entry.LogLevel)
            .ShouldBe([LogLevel.Warning]);
    }
}
