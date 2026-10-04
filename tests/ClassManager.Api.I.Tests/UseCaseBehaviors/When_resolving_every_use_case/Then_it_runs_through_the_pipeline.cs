using ClassManager.Core.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.UseCaseBehaviors.When_resolving_every_use_case;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_runs_through_the_pipeline(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_runs_through_the_pipeline_Run()
    {
        await using var scope = fixture.ApiFactory.Services.CreateAsyncScope();

        var useCasesOutsideThePipeline = typeof(IUseCase<,>).Assembly.GetTypes()
            .Where(type => !type.IsGenericTypeDefinition)
            .SelectMany(type => type.GetInterfaces())
            .Where(contract => contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IUseCase<,>))
            .Distinct()
            .Where(contract => !RunsThroughThePipeline(scope.ServiceProvider.GetService(contract)))
            .Select(contract => contract.GetGenericArguments()[0].Name);

        useCasesOutsideThePipeline.ShouldBeEmpty();
    }

    private static bool RunsThroughThePipeline(object? useCase) =>
        useCase?.GetType() is { IsGenericType: true } useCaseType
        && useCaseType.GetGenericTypeDefinition() == typeof(UseCasePipeline<,>);
}
