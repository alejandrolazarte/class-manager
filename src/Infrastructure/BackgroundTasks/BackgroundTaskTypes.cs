namespace ClassManager.Infrastructure.BackgroundTasks;

public sealed class BackgroundTaskTypes
{
    private const string UnknownCommandMessage = "Background task command is not registered: ";
    private const string UnknownTypeMessage = "Background task type is not registered: ";

    private readonly Dictionary<string, BackgroundTaskRegistration> _byTypeName;
    private readonly Dictionary<Type, BackgroundTaskRegistration> _byCommandType;

    public BackgroundTaskTypes(IEnumerable<BackgroundTaskRegistration> registrations)
    {
        var allRegistrations = registrations.ToList();
        _byTypeName = allRegistrations.ToDictionary(registration => registration.TypeName, StringComparer.Ordinal);
        _byCommandType = allRegistrations.ToDictionary(registration => registration.CommandType);
    }

    public BackgroundTaskRegistration Of(BackgroundTaskCommand command) =>
        _byCommandType.TryGetValue(command.GetType(), out var registration)
            ? registration
            : throw new InvalidOperationException(UnknownCommandMessage + command.GetType().Name);

    public BackgroundTaskRegistration Named(string typeName) =>
        _byTypeName.TryGetValue(typeName, out var registration)
            ? registration
            : throw new InvalidOperationException(UnknownTypeMessage + typeName);
}
