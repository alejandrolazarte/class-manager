namespace ClassManager.Core.Common;

public enum ErrorKind
{
    Failure,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Locked,
}

public sealed record ResultError(
    string Code,
    string Message,
    ErrorKind Kind)
{
    public string? FieldName { get; init; }

    public IReadOnlyDictionary<string, object?> Details { get; init; } = new Dictionary<string, object?>();
}
