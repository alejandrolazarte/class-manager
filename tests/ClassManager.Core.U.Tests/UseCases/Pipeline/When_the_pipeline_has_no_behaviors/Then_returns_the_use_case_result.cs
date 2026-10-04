using ClassManager.Core.UseCases;

namespace ClassManager.Core.U.Tests.UseCases.Pipeline.When_the_pipeline_has_no_behaviors;

public sealed class Then_returns_the_use_case_result
{
    [Fact]
    public async Task Then_returns_the_use_case_result_Run()
    {
        var pipeline = new UseCasePipeline<ProbeCommand, string>(new RecordingUseCase([]), []);

        var result = await pipeline.ExecuteAsync(new ProbeCommand(), CancellationToken.None);

        result.Value.ShouldBe(RecordingUseCase.Response);
    }
}
