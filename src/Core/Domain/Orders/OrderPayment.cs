using ClassManager.Core.Domain.Fees;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Orders;

public sealed class OrderPayment : ITenantOwned, IAuditable, ISoftDeletable
{
    private OrderPayment()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public DateOnly PaidOn { get; private set; }
    public long? RecordedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }

    public void Restore()
    {
        DeletedAt = null;
        DeletedByUserId = null;
        IsDeleted = false;
    }

    internal static OrderPayment Create(
        long orderId, decimal amount, PaymentMethod method, DateOnly paidOn, long? recordedByUserId, DateTimeOffset createdAt) =>
        new()
        {
            OrderId = orderId,
            Amount = amount,
            Method = method,
            PaidOn = paidOn,
            RecordedByUserId = recordedByUserId,
            CreatedAt = createdAt.ToUniversalTime(),
        };
}
