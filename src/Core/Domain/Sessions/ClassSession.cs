using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Sessions;

public sealed class ClassSession : ITenantOwned
{
    public const int CancellationReasonMaxLength = 200;

    private const string CancellationReasonLengthMessage = "The reason must be at most 200 characters.";
    private const string EndsAfterMidnightMessage = "The class must end by midnight.";
    private const string CancelledMessage = "The class is cancelled on that date.";
    private const string StartTimeFieldName = "StartTime";
    private const int MinutesPerDay = 24 * 60;

    private ClassSession()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClassGroupId { get; private set; }
    public DateOnly Date { get; private set; }
    public bool IsCancelled { get; private set; }
    public string? CancellationReason { get; private set; }
    public TimeOnly? RescheduledStartTime { get; private set; }
    public Guid? SubstituteInstructorId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static ClassSession Create(Guid classGroupId, DateOnly date, DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ClassGroupId = classGroupId,
            Date = date,
            CreatedAt = createdAt.ToUniversalTime(),
        };

    public Result Cancel(string? reason)
    {
        var trimmedReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        if (trimmedReason?.Length > CancellationReasonMaxLength)
        {
            return Result.Validation(CancellationReasonLengthMessage, fieldName: nameof(CancellationReason));
        }

        IsCancelled = true;
        CancellationReason = trimmedReason;
        return Result.Success();
    }

    public Result Reschedule(TimeOnly startTime, int durationMinutes, TimeOnly usualStartTime)
    {
        if (IsCancelled)
        {
            return Result.Conflict(CancelledMessage, SessionErrorCodes.Cancelled);
        }

        if ((startTime.Hour * 60) + startTime.Minute + durationMinutes > MinutesPerDay)
        {
            return Result.Validation(EndsAfterMidnightMessage, fieldName: StartTimeFieldName);
        }

        RescheduledStartTime = startTime == usualStartTime ? null : startTime;
        return Result.Success();
    }

    public void ClearReschedule() => RescheduledStartTime = null;

    public TimeOnly EffectiveStartTime(TimeOnly usualStartTime) => RescheduledStartTime ?? usualStartTime;

    public Result AssignSubstitute(Guid instructorId, Guid usualInstructorId)
    {
        if (IsCancelled)
        {
            return Result.Conflict(CancelledMessage, SessionErrorCodes.Cancelled);
        }

        SubstituteInstructorId = instructorId == usualInstructorId ? null : instructorId;
        return Result.Success();
    }

    public void ClearSubstitute() => SubstituteInstructorId = null;

    public Guid EffectiveInstructorId(Guid usualInstructorId) => SubstituteInstructorId ?? usualInstructorId;

    public void Restore()
    {
        IsCancelled = false;
        CancellationReason = null;
    }
}
