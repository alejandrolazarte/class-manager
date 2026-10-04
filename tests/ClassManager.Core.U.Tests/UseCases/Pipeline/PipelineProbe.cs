using ClassManager.Core.UseCases;

namespace ClassManager.Core.U.Tests.UseCases.Pipeline;

public sealed record ProbeCommand : ICommand;

internal sealed class RecordingUseCase(List<string> steps) : IUseCase<ProbeCommand, string>
{
    public const string Response = "done";
    public const string Step = "use case";

    public Task<Result<string>> ExecuteAsync(ProbeCommand command, CancellationToken cancellationToken)
    {
        steps.Add(Step);

        return Task.FromResult<Result<string>>(Response);
    }
}

internal sealed class RecordingBehavior(List<string> steps, string name) : IUseCaseBehavior<ProbeCommand, string>
{
    public static string Before(string name) => $"{name} before";

    public static string After(string name) => $"{name} after";

    public async Task<Result<string>> HandleAsync(ProbeCommand command, UseCaseStep<string> nextStep, CancellationToken cancellationToken)
    {
        steps.Add(Before(name));
        var result = await nextStep();
        steps.Add(After(name));

        return result;
    }
}

internal sealed class StoppingBehavior : IUseCaseBehavior<ProbeCommand, string>
{
    public const string ErrorCode = "stopped";
    public const string ErrorMessage = "Stopped before the use case.";

    public Task<Result<string>> HandleAsync(ProbeCommand command, UseCaseStep<string> nextStep, CancellationToken cancellationToken) =>
        Task.FromResult(Result.Failure<string>(ErrorCode, ErrorMessage));
}
