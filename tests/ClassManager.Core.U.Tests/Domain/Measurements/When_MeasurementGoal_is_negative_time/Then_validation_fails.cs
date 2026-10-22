using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.U.Tests.Domain.Measurements.When_MeasurementGoal_is_negative_time;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var goal = MeasurementGoal.Create(TestIds.Next(), MeasurementSamples.Time(), -28m);

        goal.Error!.FieldName.ShouldBe(nameof(MeasurementGoal.Value));
    }
}
