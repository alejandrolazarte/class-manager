using ClassManager.Core.Domain.Makeups;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record StudentAppMakeupCreditResponse(DateOnly MissedOn, MakeupReason Reason, DateOnly ExpiresOn);

public sealed record StudentAppClassSlotResponse(
    Guid ClassGroupId,
    string Name,
    DateOnly Date,
    string StartTime,
    string EndTime,
    string? InstructorFullName,
    string? Location,
    int SpotsLeft,
    bool IsBooked);

public sealed record StudentAppMakeupsResponse(
    IReadOnlyList<StudentAppMakeupCreditResponse> Credits,
    IReadOnlyList<StudentAppClassSlotResponse> Slots);

public sealed record StudentAppPackClassesResponse(
    int ClassesLeft,
    IReadOnlyList<StudentAppClassSlotResponse> Slots);
