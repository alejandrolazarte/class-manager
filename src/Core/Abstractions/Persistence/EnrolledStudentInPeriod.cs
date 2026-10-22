namespace ClassManager.Core.Abstractions.Persistence;

public sealed record EnrolledStudentInPeriod(
    long ClientId,
    string ClientFullName,
    string ClientPhoneNumber,
    string StudentFullName);
