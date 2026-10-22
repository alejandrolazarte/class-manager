using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.U.Tests.Domain.Measurements.When_MeasurementDefinition_is_hidden_from_students;

public sealed class Then_students_can_no_longer_record_it
{
    [Fact]
    public void Then_students_can_no_longer_record_it_Run()
    {
        var definition = MeasurementDefinition.Create(
            "Nado continuo", MeasurementKind.Distance, "m", ImprovementDirection.Increases, isVisibleToStudents: true, isRecordedByStudents: true, TestData.Now).Value!.WithId();

        var result = definition.Update("Nado continuo", ImprovementDirection.Increases, isVisibleToStudents: false, isRecordedByStudents: true);

        result.Error!.FieldName.ShouldBe(nameof(MeasurementDefinition.IsRecordedByStudents));
    }
}
