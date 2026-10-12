using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.UseCases.Measurements;

public sealed record MeasurementResponse(
    Guid Id,
    string Name,
    MeasurementKind Kind,
    string Unit,
    ImprovementDirection ImprovesWhen,
    bool IsVisibleToStudents,
    bool IsRecordedByStudents)
{
    public static MeasurementResponse From(MeasurementDefinition definition) =>
        new(
            definition.Id,
            definition.Name,
            definition.Kind,
            definition.Unit,
            definition.ImprovesWhen,
            definition.IsVisibleToStudents,
            definition.IsRecordedByStudents);
}

public sealed record MeasurementRecordResponse(
    Guid Id,
    Guid StudentId,
    Guid MeasurementId,
    DateOnly MeasuredOn,
    decimal Value,
    string? Note,
    bool RecordedByStudent,
    string? RecordedByFullName)
{
    public static MeasurementRecordResponse From(MeasurementRecord record, string? recordedByFullName = null) =>
        new(
            record.Id,
            record.StudentId,
            record.MeasurementDefinitionId,
            record.MeasuredOn,
            record.Value,
            record.Note,
            record.RecordedByStudent,
            recordedByFullName);
}

public sealed record StudentMeasurementResponse(
    MeasurementResponse Measurement,
    decimal? Goal,
    IReadOnlyList<MeasurementRecordResponse> Records);

internal static class StudentMeasurements
{
    public static IReadOnlyList<StudentMeasurementResponse> For(
        Guid studentId,
        IEnumerable<MeasurementDefinition> definitions,
        IReadOnlyList<MeasurementRecord> records,
        IReadOnlyList<MeasurementGoal> goals,
        IReadOnlyDictionary<Guid, string> recorderNames) =>
    [
        .. definitions.Select(definition => new StudentMeasurementResponse(
            MeasurementResponse.From(definition),
            goals.FirstOrDefault(goal => goal.StudentId == studentId && goal.MeasurementDefinitionId == definition.Id)?.Value,
            [
                .. records
                    .Where(record => record.StudentId == studentId && record.MeasurementDefinitionId == definition.Id)
                    .OrderBy(record => record.MeasuredOn)
                    .Select(record => MeasurementRecordResponse.From(
                        record,
                        record.RecordedByUserId is { } userId ? recorderNames.GetValueOrDefault(userId) : null)),
            ])),
    ];
}
