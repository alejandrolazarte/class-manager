using System.Collections.Frozen;

namespace ClassManager.Core.Domain.Measurements;

public static class MeasurementUnits
{
    public const int MaxLength = 12;
    public const string Seconds = "s";
    public const string Kilograms = "kg";
    public const string Pounds = "lb";
    public const string Meters = "m";
    public const string Kilometers = "km";
    public const string Centimeters = "cm";
    public const string Repetitions = "rep";

    private static readonly FrozenDictionary<MeasurementKind, string[]> UnitsByKind =
        new Dictionary<MeasurementKind, string[]>
        {
            [MeasurementKind.Time] = [Seconds],
            [MeasurementKind.Weight] = [Kilograms, Pounds],
            [MeasurementKind.Distance] = [Meters, Kilometers],
            [MeasurementKind.Length] = [Centimeters],
            [MeasurementKind.Repetitions] = [Repetitions],
        }.ToFrozenDictionary();

    public static bool Fits(MeasurementKind kind, string unit) =>
        kind == MeasurementKind.Number
            ? unit.Length is > 0 and <= MaxLength
            : UnitsByKind.TryGetValue(kind, out var units) && units.Contains(unit, StringComparer.Ordinal);
}
