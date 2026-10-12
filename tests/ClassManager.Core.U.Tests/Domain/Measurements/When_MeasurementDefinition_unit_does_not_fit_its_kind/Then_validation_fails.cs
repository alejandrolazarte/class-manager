using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.U.Tests.Domain.Measurements.When_MeasurementDefinition_unit_does_not_fit_its_kind;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var definition = MeasurementDefinition.Create(
            "Sentadilla", MeasurementKind.Weight, "m", ImprovementDirection.Increases, isVisibleToStudents: true, isRecordedByStudents: false, TestData.Now);

        definition.Error!.FieldName.ShouldBe(nameof(MeasurementDefinition.Unit));
    }
}
