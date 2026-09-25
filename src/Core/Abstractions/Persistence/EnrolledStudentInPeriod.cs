namespace ClassManager.Core.Abstractions.Persistence;

public sealed record EnrolledStudentInPeriod(
    Guid ClientId,
    string ClientFullName,
    string ClientPhoneNumber,
    decimal? ClientMonthlyFee,
    string StudentFullName);
