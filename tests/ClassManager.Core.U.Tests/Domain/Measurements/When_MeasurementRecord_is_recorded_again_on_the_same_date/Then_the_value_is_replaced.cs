using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.U.Tests.Domain.Measurements.When_MeasurementRecord_is_recorded_again_on_the_same_date;

public sealed class Then_the_value_is_replaced
{
    [Fact]
    public void Then_the_value_is_replaced_Run()
    {
        var definition = MeasurementSamples.Time();
        var record = MeasurementRecord.Create(
            TestIds.Next(), definition, TestData.Today, 30.9m, "Con aletas", TestData.Today, TestData.Now, recordedByUserId: null, recordedByStudent: false).Value!.WithId();

        record.Replace(definition, 29.7m, null, recordedByUserId: null, recordedByStudent: true);

        record.Value.ShouldBe(29.7m);
        record.RecordedByStudent.ShouldBeTrue();
    }
}
