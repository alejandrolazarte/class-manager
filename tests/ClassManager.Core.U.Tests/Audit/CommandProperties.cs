using System.Collections;
using System.Reflection;
using ClassManager.Core.Audit;
using ClassManager.Core.UseCases;

namespace ClassManager.Core.U.Tests.Audit;

internal static class CommandProperties
{
    private static readonly Type[] AuditAttributes = [typeof(AuditSecretAttribute), typeof(AuditPersonalAttribute), typeof(AuditPlainAttribute)];

    public static IReadOnlyList<PropertyInfo> All()
    {
        var coreAssembly = typeof(ICommand).Assembly;
        var visited = new HashSet<Type>();
        var properties = new List<PropertyInfo>();
        foreach (var commandType in coreAssembly.GetTypes().Where(type => typeof(ICommand).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false }))
        {
            Collect(commandType, coreAssembly, visited, properties);
        }

        return properties;
    }

    public static bool IsText(PropertyInfo property) => ElementTypeOf(property.PropertyType) == typeof(string);

    public static IReadOnlyList<Type> AuditAttributesOf(PropertyInfo property) =>
        [.. AuditAttributes.Where(attribute => property.IsDefined(attribute, inherit: false))];

    public static string NameOf(PropertyInfo property) => $"{property.DeclaringType!.Name}.{property.Name}";

    private static void Collect(Type type, Assembly coreAssembly, HashSet<Type> visited, List<PropertyInfo> properties)
    {
        if (!visited.Add(type))
        {
            return;
        }

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            properties.Add(property);
            var elementType = ElementTypeOf(property.PropertyType);
            if (elementType.Assembly == coreAssembly && !elementType.IsEnum)
            {
                Collect(elementType, coreAssembly, visited, properties);
            }
        }
    }

    private static Type ElementTypeOf(Type propertyType)
    {
        var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        if (type == typeof(string))
        {
            return type;
        }

        if (type.IsArray)
        {
            return type.GetElementType()!;
        }

        return type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type) ? type.GetGenericArguments()[0] : type;
    }
}
