using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases;
using ClassManager.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.UseCaseBehaviors;

public sealed record ProbeCommand : ICommand;

public sealed record ProbeQuery : IQuery;

internal static class UseCaseBehaviorProbe
{
    public const string FailureCode = "probe_failed";
    public const string FailureMessage = "The probe use case failed.";
    public const string ThrownMessage = "The probe use case threw.";

    public static Task<Result<bool>> RunAsync<TInput>(IServiceProvider services, TInput input, Func<Task<Result<bool>>> execute) =>
        new UseCasePipeline<TInput, bool>(new DelegateUseCase<TInput>(execute), services.GetServices<IUseCaseBehavior<TInput, bool>>())
            .ExecuteAsync(input, CancellationToken.None);

    public static Task<Result<bool>> Succeeded() => Task.FromResult(Result.Success(true));

    public static Result<bool> Failed() => Result.Failure<bool>(FailureCode, FailureMessage);

    public static AsyncServiceScope BusinessScope(ApiFixture fixture, Guid businessId)
    {
        var scope = fixture.ApiFactory.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ITenantScope>().Establish(businessId);

        return scope;
    }

    public static Client NewClient(SeededBusiness business)
    {
        var phoneNumber = PhoneNumber.Create(ApiRequests.ClientPhoneNumber, business.Business.DefaultCountryCallingCode).Value!;

        return Client.Create(ApiRequests.ClientFullName, phoneNumber, null, null, BusinessApiFactory.Now).Value!;
    }

    public static async Task<bool> ClientExistsAsync(ApiFixture fixture, Guid businessId, Guid clientId)
    {
        await using var context = fixture.CreateDbContext(businessId);

        return await context.Clients.AnyAsync(saved => saved.Id == clientId);
    }

    private sealed class DelegateUseCase<TInput>(Func<Task<Result<bool>>> execute) : IUseCase<TInput, bool>
    {
        public Task<Result<bool>> ExecuteAsync(TInput command, CancellationToken cancellationToken) => execute();
    }
}
