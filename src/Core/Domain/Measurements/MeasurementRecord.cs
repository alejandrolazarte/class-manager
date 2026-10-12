using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Measurements;

public sealed class MeasurementRecord : ITenantOwned, ISoftDeletable
{
    public const int NoteMaxLength = 200;

    private const string MeasuredOnMessage = "The date can't be in the future.";
    private const string NoteLengthMessage = "The note must be at most 200 characters.";

    private static readonly DateOnly EarliestMeasuredOn = new(1900, 1, 1);

    private MeasurementRecord()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid MeasurementDefinitionId { get; private set; }
    public DateOnly MeasuredOn { get; private set; }
    public decimal Value { get; private set; }
    public string? Note { get; private set; }
    public Guid? RecordedByUserId { get; private set; }
    public bool RecordedByStudent { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Result<MeasurementRecord> Create(
        Guid studentId,
        MeasurementDefinition definition,
        DateOnly measuredOn,
        decimal? value,
        string? note,
        DateOnly today,
        DateTimeOffset createdAt,
        Guid? recordedByUserId,
        bool recordedByStudent)
    {
        if (measuredOn > today || measuredOn < EarliestMeasuredOn)
        {
            return Result.Validation<MeasurementRecord>(MeasuredOnMessage, fieldName: nameof(MeasuredOn));
        }

        var record = new MeasurementRecord
        {
            Id = Guid.CreateVersion7(),
            StudentId = studentId,
            MeasurementDefinitionId = definition.Id,
            MeasuredOn = measuredOn,
            CreatedAt = createdAt.ToUniversalTime(),
        };
        var replaced = record.Replace(definition, value, note, recordedByUserId, recordedByStudent);
        return replaced.IsFailure ? replaced.Error! : record;
    }

    public Result Replace(MeasurementDefinition definition, decimal? value, string? note, Guid? recordedByUserId, bool recordedByStudent)
    {
        var validValue = MeasurementValue.Validate(definition.Kind, value, nameof(Value));
        if (validValue.IsFailure)
        {
            return Result.Failure(validValue.Error!);
        }

        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote?.Length > NoteMaxLength)
        {
            return Result.Validation(NoteLengthMessage, fieldName: nameof(Note));
        }

        Value = validValue.Value;
        Note = trimmedNote;
        RecordedByUserId = recordedByUserId;
        RecordedByStudent = recordedByStudent;
        return Result.Success();
    }

    public void Delete(DateTimeOffset deletedOn)
    {
        DeletedOn = DeletedOnGuard.Delete(DeletedOn, deletedOn);
        IsDeleted = true;
    }

    public void Restore()
    {
        DeletedOn = null;
        IsDeleted = false;
    }
}
