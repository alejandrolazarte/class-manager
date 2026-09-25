namespace ClassManager.Core.Common;

public class Result
{
    public const string ValidationCode = "validation";
    public const string NotFoundCode = "not_found";
    public const string ConflictCode = "conflict";
    public const string UnauthorizedCode = "unauthorized";
    public const string LockedCode = "locked";

    private protected Result(ResultError? error)
    {
        Error = error;
    }

    public bool IsSuccess => Error is null;
    public bool IsFailure => !IsSuccess;
    public ResultError? Error { get; }

    public static Result Success() => new(null);

    public static Result Failure(ResultError error) => new(error);

    public static Result Failure(string code, string message) =>
        new(new ResultError(code, message, ErrorKind.Failure));

    public static Result Validation(string message, string code = ValidationCode, string? fieldName = null) =>
        new(new ResultError(code, message, ErrorKind.Validation) { FieldName = fieldName });

    public static Result NotFound(string message, string code = NotFoundCode) =>
        new(new ResultError(code, message, ErrorKind.NotFound));

    public static Result Conflict(string message, string code = ConflictCode) =>
        new(new ResultError(code, message, ErrorKind.Conflict));

    public static Result Unauthorized(string message, string code = UnauthorizedCode) =>
        new(new ResultError(code, message, ErrorKind.Unauthorized));

    public static Result<T> Success<T>(T value) => new(value, null);

    public static Result<T> Failure<T>(ResultError error) => new(default, error);

    public static Result<T> Failure<T>(string code, string message) =>
        new(default, new ResultError(code, message, ErrorKind.Failure));

    public static Result<T> Validation<T>(string message, string code = ValidationCode, string? fieldName = null) =>
        new(default, new ResultError(code, message, ErrorKind.Validation) { FieldName = fieldName });

    public static Result<T> NotFound<T>(string message, string code = NotFoundCode) =>
        new(default, new ResultError(code, message, ErrorKind.NotFound));

    public static Result<T> Conflict<T>(
        string message,
        string code = ConflictCode,
        IReadOnlyDictionary<string, object?>? details = null) =>
        new(default, new ResultError(code, message, ErrorKind.Conflict) { Details = details ?? new Dictionary<string, object?>() });

    public static Result<T> Unauthorized<T>(string message, string code = UnauthorizedCode) =>
        new(default, new ResultError(code, message, ErrorKind.Unauthorized));

    public static Result<T> Locked<T>(string message, string code = LockedCode) =>
        new(default, new ResultError(code, message, ErrorKind.Locked));
}

public sealed class Result<T> : Result
{
    internal Result(T? value, ResultError? error) : base(error)
    {
        Value = value;
    }

    public T? Value { get; }

    public static implicit operator Result<T>(T value) => new(value, null);

    public static implicit operator Result<T>(ResultError error) => new(default, error);

    public TOut Resolve<TOut>(
        Func<T, TOut> onSuccess,
        Func<ResultError, TOut> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    }
}
