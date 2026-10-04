using ClassManager.Core.UseCases;

namespace ClassManager.Core.U.Tests.UseCases.Pipeline.When_the_pipeline_has_behaviors;

public sealed class Then_they_run_in_order_around_the_use_case
{
    private const string OuterBehavior = "outer";
    private const string InnerBehavior = "inner";

    [Fact]
    public async Task Then_they_run_in_order_around_the_use_case_Run()
    {
        List<string> steps = [];
        var pipeline = new UseCasePipeline<ProbeCommand, string>(
            new RecordingUseCase(steps),
            [new RecordingBehavior(steps, OuterBehavior), new RecordingBehavior(steps, InnerBehavior)]);

        await pipeline.ExecuteAsync(new ProbeCommand(), CancellationToken.None);

        steps.ShouldBe(
        [
            RecordingBehavior.Before(OuterBehavior),
            RecordingBehavior.Before(InnerBehavior),
            RecordingUseCase.Step,
            RecordingBehavior.After(InnerBehavior),
            RecordingBehavior.After(OuterBehavior),
        ]);
    }
}
