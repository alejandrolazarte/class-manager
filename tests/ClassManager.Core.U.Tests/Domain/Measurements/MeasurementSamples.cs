using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.U.Tests.Domain.Measurements;

internal static class MeasurementSamples
{
    public static MeasurementDefinition Time() => Create("25 m crol", MeasurementKind.Time, "s", ImprovementDirection.Decreases);

    public static MeasurementDefinition Length() => Create("Flexión hacia adelante", MeasurementKind.Length, "cm", ImprovementDirection.Increases);

    public static MeasurementDefinition Repetitions() => Create("Dominadas", MeasurementKind.Repetitions, "rep", ImprovementDirection.Increases);

    private static MeasurementDefinition Create(string name, MeasurementKind kind, string unit, ImprovementDirection improvesWhen) =>
        MeasurementDefinition.Create(name, kind, unit, improvesWhen, isVisibleToStudents: true, isRecordedByStudents: false, TestData.Now).Value!.WithId();
}
