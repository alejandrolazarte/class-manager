using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Enrollments;

public sealed class Enrollment : ITenantOwned
{
    private const string EndBeforeStartMessage = "The end date can't be before the start date.";
    private const string AlreadyEndedMessage = "The enrollment has already ended.";

    private Enrollment()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid ClassGroupId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Enrollment Create(Guid studentId, Guid classGroupId, DateOnly startDate, DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.CreateVersion7(),
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
