using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Measurements;

public sealed class MeasurementGoal : ITenantOwned
{
    private MeasurementGoal()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid MeasurementDefinitionId { get; private set; }
    public decimal Value { get; private set; }

    public static Result<MeasurementGoal> Create(Guid studentId, MeasurementDefinition definition, decimal? value)
    {
        var goal = new MeasurementGoal
        {
            Id = Guid.CreateVersion7(),
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
