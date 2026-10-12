using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Measurements;

public sealed class MeasurementDefinition : ITenantOwned, ISoftDeletable
{
    public const int NameMaxLength = 60;

    private const string NameRequiredMessage = "Write a name.";
    private const string NameLengthMessage = "The name must be at most 60 characters.";
    private const string KindMessage = "Choose a kind of value.";
    private const string UnitMessage = "Choose a unit that fits the kind of value.";
    private const string ImprovesWhenMessage = "Choose when the value improves.";
    private const string RecordedByStudentsMessage = "Students can only record a measurement they can see.";

    private MeasurementDefinition()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public MeasurementKind Kind { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public ImprovementDirection ImprovesWhen { get; private set; }
    public bool IsVisibleToStudents { get; private set; }
    public bool IsRecordedByStudents { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Result<MeasurementDefinition> Create(
        string? name,
        MeasurementKind? kind,
        string? unit,
        ImprovementDirection? improvesWhen,
        bool isVisibleToStudents,
        bool isRecordedByStudents,
        DateTimeOffset createdAt)
    {
        if (kind is null || !Enum.IsDefined(kind.Value))
        {
            return Result.Validation<MeasurementDefinition>(KindMessage, fieldName: nameof(Kind));
        }

        var trimmedUnit = unit?.Trim() ?? string.Empty;
        if (!MeasurementUnits.Fits(kind.Value, trimmedUnit))
        {
            return Result.Validation<MeasurementDefinition>(UnitMessage, fieldName: nameof(Unit));
        }

        var definition = new MeasurementDefinition
        {
            Id = Guid.CreateVersion7(),
            Kind = kind.Value,
            Unit = trimmedUnit,
            CreatedAt = createdAt.ToUniversalTime(),
        };
        var updated = definition.Update(name, improvesWhen, isVisibleToStudents, isRecordedByStudents);
        return updated.IsFailure ? updated.Error! : definition;
    }

    public Result Update(string? name, ImprovementDirection? improvesWhen, bool isVisibleToStudents, bool isRecordedByStudents)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedName.Length == 0)
        {
            return Result.Validation(NameRequiredMessage, fieldName: nameof(Name));
        }

        if (trimmedName.Length > NameMaxLength)
        {
            return Result.Validation(NameLengthMessage, fieldName: nameof(Name));
        }

        if (improvesWhen is null || !Enum.IsDefined(improvesWhen.Value))
        {
            return Result.Validation(ImprovesWhenMessage, fieldName: nameof(ImprovesWhen));
        }

        if (isRecordedByStudents && !isVisibleToStudents)
        {
            return Result.Validation(RecordedByStudentsMessage, fieldName: nameof(IsRecordedByStudents));
        }

        Name = trimmedName;
        ImprovesWhen = improvesWhen.Value;
        IsVisibleToStudents = isVisibleToStudents;
        IsRecordedByStudents = isRecordedByStudents;
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
