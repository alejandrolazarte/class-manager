using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ClassManager.Core.Audit;

namespace ClassManager.Infrastructure.Audit;

public static class CommandAuditPayload
{
    public const string PersonalMarker = "[personal]";
    public const int MaximumLength = 32 * 1024;

    private const string FileMarker = "[file]";

    private static readonly CompositeFormat FileSizeFormat = CompositeFormat.Parse("[file: {0} bytes]");

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
        TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { HideSecretAndPersonalFields } },
    };

    public static (string Json, bool Truncated) Serialize(object command)
    {
        var json = JsonSerializer.Serialize(command, command.GetType(), Options);
        return json.Length > MaximumLength ? (json[..MaximumLength], true) : (json, false);
    }

    private static void HideSecretAndPersonalFields(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        for (var index = typeInfo.Properties.Count - 1; index >= 0; index--)
        {
            var property = typeInfo.Properties[index];
            if (IsMarked<AuditSecretAttribute>(property))
            {
                typeInfo.Properties.RemoveAt(index);
            }
            else if (IsMarked<AuditPersonalAttribute>(property))
            {
                typeInfo.Properties[index] = TextProperty(typeInfo, property, _ => PersonalMarker);
            }
            else if (IsFile(property.PropertyType))
            {
                typeInfo.Properties[index] = TextProperty(typeInfo, property, DescribeFile);
            }
        }
    }

    private static JsonPropertyInfo TextProperty(JsonTypeInfo typeInfo, JsonPropertyInfo property, Func<object, string> describe)
    {
        var getValue = property.Get!;
        var textProperty = typeInfo.CreateJsonPropertyInfo(typeof(string), property.Name);
        textProperty.Get = owner => getValue(owner) is { } value ? describe(value) : null;
        return textProperty;
    }

    private static bool IsMarked<TAttribute>(JsonPropertyInfo property)
        where TAttribute : Attribute =>
        property.AttributeProvider is MemberInfo member && member.IsDefined(typeof(TAttribute), inherit: false);

    private static bool IsFile(Type type) => type == typeof(byte[]) || typeof(Stream).IsAssignableFrom(type);

    private static string DescribeFile(object file) => file switch
    {
        byte[] bytes => string.Format(CultureInfo.InvariantCulture, FileSizeFormat, bytes.Length),
        Stream { CanSeek: true } stream => string.Format(CultureInfo.InvariantCulture, FileSizeFormat, stream.Length),
        _ => FileMarker,
    };
}
