using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.U.Tests.Domain.Measurements.When_MeasurementRecord_has_more_than_three_decimals;

public sealed class Then_the_value_is_rounded
{
    [Fact]
    public void Then_the_value_is_rounded_Run()
    {
        var record = MeasurementRecord.Create(
            Guid.NewGuid(), MeasurementSamples.Time(), TestData.Today, 29.7049m, null, TestData.Today, TestData.Now, recordedByUserId: null, recordedByStudent: false);

        record.Value!.Value.ShouldBe(29.705m);
    }
}
