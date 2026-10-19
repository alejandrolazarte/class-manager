using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ClassManager.Core.Audit;
using ClassManager.Core.UseCases;

namespace ClassManager.Api.I.Tests.Audit;

internal sealed class CommandSamples
{
    private const string PlainText = "plain";
    private const int MaximumDepth = 5;

    private readonly List<string> _hiddenValues = [];
    private int _nextHiddenValue;

    public IReadOnlyList<string> HiddenValues => _hiddenValues;

    public static IReadOnlyList<Type> CommandTypes() =>
        [.. typeof(ICommand).Assembly.GetTypes().Where(type => typeof(ICommand).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false })];

    public object Build(Type type) => BuildObject(type, depth: 0);

    private object BuildObject(Type type, int depth)
    {
        var instance = RuntimeHelpers.GetUninitializedObject(type);
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var setter = property.GetSetMethod(nonPublic: true);
            if (setter is null)
            {
                continue;
            }

            var hidden = property.IsDefined(typeof(AuditSecretAttribute)) || property.IsDefined(typeof(AuditPersonalAttribute));
            setter.Invoke(instance, [ValueOf(property.PropertyType, hidden, depth)]);
        }

        return instance;
    }

    private object? ValueOf(Type propertyType, bool hidden, int depth)
    {
        var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        if (type == typeof(string))
        {
            return hidden ? Hide($"leak-{_nextHiddenValue++}") : PlainText;
        }

        if (type == typeof(byte[]))
        {
            return FileBytes();
        }

        if (typeof(Stream).IsAssignableFrom(type))
        {
            return new MemoryStream(FileBytes());
        }

        if (type == typeof(DateOnly))
        {
            var date = new DateOnly(1901, 1, 1).AddDays(_nextHiddenValue++);
            return hidden ? Hide(date, date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)) : new DateOnly(2026, 1, 1);
        }

        if (type == typeof(decimal))
        {
            var value = 987654.321m + _nextHiddenValue++;
            return hidden ? Hide(value, value.ToString(CultureInfo.InvariantCulture)) : 1m;
        }

        if (type == typeof(Guid))
        {
            return Guid.CreateVersion7();
        }

        if (type == typeof(DateTimeOffset))
        {
            return new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        }

        if (type.IsEnum || type.IsPrimitive || type == typeof(TimeOnly))
        {
            return Activator.CreateInstance(type);
        }

        if (type.IsArray || (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type)))
        {
            var elementType = type.IsArray ? type.GetElementType()! : type.GetGenericArguments()[0];
            var elements = Array.CreateInstance(elementType, 1);
            elements.SetValue(ValueOf(elementType, hidden, depth + 1), 0);
            return type.IsAssignableFrom(elements.GetType()) ? elements : Activator.CreateInstance(type, elements);
        }

        return depth < MaximumDepth ? BuildObject(type, depth + 1) : null;
    }

    private string Hide(string value)
    {
        _hiddenValues.Add(value);
        return value;
    }

    private T Hide<T>(T value, string text)
    {
        _hiddenValues.Add(text);
        return value;
    }

    private byte[] FileBytes()
    {
        var bytes = Encoding.UTF8.GetBytes($"leak-file-content-{_nextHiddenValue++}");
        _hiddenValues.Add(Convert.ToBase64String(bytes));
        return bytes;
    }
}
