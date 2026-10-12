using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.U.Tests.Domain.Measurements.When_MeasurementDefinition_is_recorded_by_students_but_hidden_from_them;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var definition = MeasurementDefinition.Create(
            "Nado continuo", MeasurementKind.Distance, "m", ImprovementDirection.Increases, isVisibleToStudents: false, isRecordedByStudents: true, TestData.Now);

        definition.Error!.FieldName.ShouldBe(nameof(MeasurementDefinition.IsRecordedByStudents));
    }
}
