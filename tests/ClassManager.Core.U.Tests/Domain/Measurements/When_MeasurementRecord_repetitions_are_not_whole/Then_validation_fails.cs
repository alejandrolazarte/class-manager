using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.U.Tests.Domain.Measurements.When_MeasurementRecord_repetitions_are_not_whole;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var record = MeasurementRecord.Create(
            Guid.NewGuid(), MeasurementSamples.Repetitions(), TestData.Today, 4.5m, null, TestData.Today, TestData.Now, recordedByUserId: null, recordedByStudent: false);

        record.Error!.FieldName.ShouldBe(nameof(MeasurementRecord.Value));
    }
}
