using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Enrollments;

public sealed class Enrollment : ITenantOwned, IAuditable
{
    private const string EndBeforeStartMessage = "The end date can't be before the start date.";
    private const string AlreadyEndedMessage = "The enrollment has already ended.";

    private Enrollment()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long StudentId { get; private set; }
    public long ClassGroupId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }

    public static Enrollment Create(long studentId, long classGroupId, DateOnly startDate, DateTimeOffset createdAt) =>
        new()
        {
            StudentId = studentId,
            ClassGroupId = classGroupId,
            StartDate = startDate,
            CreatedAt = createdAt.ToUniversalTime(),
        };

    public Result End(DateOnly endDate)
    {
        if (EndDate is not null)
        {
            return Result.Conflict(AlreadyEndedMessage, EnrollmentErrorCodes.AlreadyEnded);
        }

        if (endDate < StartDate)
        {
            return Result.Validation(EndBeforeStartMessage, fieldName: nameof(EndDate));
        }

        EndDate = endDate;
        return Result.Success();
    }

    public void Continue() => EndDate = null;

    public bool IsCurrentOn(DateOnly date) => EndDate is null || EndDate >= date;

    public bool IsActiveOn(DateOnly date) => StartDate <= date && IsCurrentOn(date);
}
