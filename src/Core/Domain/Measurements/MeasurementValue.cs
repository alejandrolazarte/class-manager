using System.Collections.Frozen;

using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Measurements;

public static class MeasurementValue
{
    public const int Precision = 12;
    public const int Decimals = 3;
    public const decimal Largest = 999_999_999.999m;

    private const string RequiredMessage = "Write a value.";
    private const string RangeMessage = "The value is out of range.";
    private const string NegativeMessage = "The value can't be negative.";
    private const string WholeMessage = "Repetitions must be a whole number.";

    private static readonly FrozenSet<MeasurementKind> KindsThatCanBeNegative =
        new[] { MeasurementKind.Length, MeasurementKind.Number }.ToFrozenSet();

    public static Result<decimal> Validate(MeasurementKind kind, decimal? value, string fieldName)
    {
        if (value is null)
        {
            return Result.Validation<decimal>(RequiredMessage, fieldName: fieldName);
        }

        var rounded = decimal.Round(value.Value, Decimals, MidpointRounding.AwayFromZero);
        if (Math.Abs(rounded) > Largest)
        {
            return Result.Validation<decimal>(RangeMessage, fieldName: fieldName);
        }

        if (rounded < 0 && !KindsThatCanBeNegative.Contains(kind))
        {
            return Result.Validation<decimal>(NegativeMessage, fieldName: fieldName);
        }

        if (kind == MeasurementKind.Repetitions && rounded != decimal.Truncate(rounded))
        {
            return Result.Validation<decimal>(WholeMessage, fieldName: fieldName);
        }

        return rounded;
    }
}
