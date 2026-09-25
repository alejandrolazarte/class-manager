using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Sessions;

public sealed class ClassSession : ITenantOwned
{
    public const int CancellationReasonMaxLength = 200;

    private const string CancellationReasonLengthMessage = "The reason must be at most 200 characters.";

    private ClassSession()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClassGroupId { get; private set; }
    public DateOnly Date { get; private set; }
    public bool IsCancelled { get; private set; }
    public string? CancellationReason { get; private set; }
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

    public void Restore()
    {
        IsCancelled = false;
        CancellationReason = null;
    }
}
