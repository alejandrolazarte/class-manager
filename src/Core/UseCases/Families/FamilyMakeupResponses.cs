using ClassManager.Core.Domain.Makeups;

namespace ClassManager.Core.UseCases.Families;

public sealed record FamilyMakeupCreditResponse(DateOnly MissedOn, MakeupReason Reason, DateOnly ExpiresOn);

public sealed record FamilyClassSlotResponse(
    Guid ClassGroupId,
    string Name,
    DateOnly Date,
    string StartTime,
    string EndTime,
    string? InstructorFullName,
    string? Location,
    int SpotsLeft,
    bool IsBooked);

public sealed record FamilyMakeupsResponse(
    IReadOnlyList<FamilyMakeupCreditResponse> Credits,
    IReadOnlyList<FamilyClassSlotResponse> Slots);

public sealed record FamilyPackClassesResponse(
    int ClassesLeft,
    IReadOnlyList<FamilyClassSlotResponse> Slots);
