using ClassManager.Core.UseCases;

namespace ClassManager.Core.U.Tests.UseCases.Pipeline.When_scanning_the_use_cases;

public sealed class Then_each_input_is_either_a_command_or_a_query
{
    [Fact]
    public void Then_each_input_is_either_a_command_or_a_query_Run()
    {
        var inputsWithoutOneKind = typeof(IUseCase<,>).Assembly.GetTypes()
            .Where(type => !type.IsGenericTypeDefinition)
            .SelectMany(type => type.GetInterfaces())
            .Where(contract => contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IUseCase<,>))
            .Select(contract => contract.GetGenericArguments()[0])
            .Distinct()
            .Where(input => typeof(ICommand).IsAssignableFrom(input) == typeof(IQuery).IsAssignableFrom(input))
            .Select(input => input.Name);

        inputsWithoutOneKind.ShouldBeEmpty();
    }
}
