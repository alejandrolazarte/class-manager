using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Measurements;

public sealed class MeasurementGoal : ITenantOwned, IAuditable
{
    private MeasurementGoal()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long StudentId { get; private set; }
    public long MeasurementDefinitionId { get; private set; }
    public decimal Value { get; private set; }

    public static Result<MeasurementGoal> Create(long studentId, MeasurementDefinition definition, decimal? value)
    {
        var goal = new MeasurementGoal
        {
            StudentId = studentId,
            MeasurementDefinitionId = definition.Id,
        };
        var changed = goal.Change(definition, value);
        return changed.IsFailure ? changed.Error! : goal;
    }

    public Result Change(MeasurementDefinition definition, decimal? value)
    {
        var validValue = MeasurementValue.Validate(definition.Kind, value, nameof(Value));
        if (validValue.IsFailure)
        {
            return Result.Failure(validValue.Error!);
        }

        Value = validValue.Value;
        return Result.Success();
    }
}
