using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.U.Tests.Domain.Measurements.When_MeasurementDefinition_of_kind_number_has_its_own_unit;

public sealed class Then_the_unit_is_kept
{
    [Fact]
    public void Then_the_unit_is_kept_Run()
    {
        var definition = MeasurementDefinition.Create(
            "Dolor lumbar", MeasurementKind.Number, " pts ", ImprovementDirection.Decreases, isVisibleToStudents: false, isRecordedByStudents: false, TestData.Now);

        definition.Value!.Unit.ShouldBe("pts");
    }
}
