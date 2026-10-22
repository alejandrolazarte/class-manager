using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.U.Tests.Domain.Measurements.When_MeasurementRecord_length_is_negative;

public sealed class Then_it_is_recorded
{
    [Fact]
    public void Then_it_is_recorded_Run()
    {
        var record = MeasurementRecord.Create(
            TestIds.Next(), MeasurementSamples.Length(), TestData.Today, -8m, null, TestData.Today, TestData.Now, recordedByUserId: null, recordedByStudent: false);

        record.Value!.Value.ShouldBe(-8m);
    }
}
