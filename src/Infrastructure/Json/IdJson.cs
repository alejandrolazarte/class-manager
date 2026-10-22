using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ClassManager.Infrastructure.Json;

public static class IdJson
{
    private const string IdName = "id";
    private const string IdSuffix = "Id";
    private const string IdsSuffix = "Ids";
    private const JsonNumberHandling IdNumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;

    public static JsonSerializerOptions WriteIdsAsStrings(this JsonSerializerOptions options)
    {
        options.TypeInfoResolver = (options.TypeInfoResolver ?? new DefaultJsonTypeInfoResolver()).WithAddedModifier(WriteIdPropertiesAsStrings);
        return options;
    }

    public static IDictionary<string, object?> WithIdsAsStrings(this IReadOnlyDictionary<string, object?> extensions) =>
        extensions.ToDictionary(
            extension => extension.Key,
            extension => IsIdName(extension.Key) && extension.Value is long id ? id.ToString(CultureInfo.InvariantCulture) : extension.Value);

    private static void WriteIdPropertiesAsStrings(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        foreach (var property in typeInfo.Properties.Where(property => IsIdName(property.Name) && HoldsIds(property.PropertyType)))
        {
            property.NumberHandling = IdNumberHandling;
        }
    }

    private static bool IsIdName(string name) =>
        name == IdName || name.EndsWith(IdSuffix, StringComparison.Ordinal) || name.EndsWith(IdsSuffix, StringComparison.Ordinal);

    private static bool HoldsIds(Type type) =>
        IsId(type) || (type != typeof(string) && type.GetInterfaces().Append(type).Any(IsEnumerableOfIds));

    private static bool IsEnumerableOfIds(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) && IsId(type.GetGenericArguments()[0]);

    private static bool IsId(Type type) => type == typeof(long) || type == typeof(long?);
}
