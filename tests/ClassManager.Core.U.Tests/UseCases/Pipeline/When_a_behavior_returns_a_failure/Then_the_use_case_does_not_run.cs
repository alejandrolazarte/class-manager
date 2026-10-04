using ClassManager.Core.UseCases;

namespace ClassManager.Core.U.Tests.UseCases.Pipeline.When_a_behavior_returns_a_failure;

public sealed class Then_the_use_case_does_not_run
{
    [Fact]
    public async Task Then_the_use_case_does_not_run_Run()
    {
        List<string> steps = [];
        var pipeline = new UseCasePipeline<ProbeCommand, string>(new RecordingUseCase(steps), [new StoppingBehavior()]);

        var result = await pipeline.ExecuteAsync(new ProbeCommand(), CancellationToken.None);

        steps.ShouldBeEmpty();
        result.Error!.Code.ShouldBe(StoppingBehavior.ErrorCode);
    }
}
