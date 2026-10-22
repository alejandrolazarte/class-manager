using ClassManager.Core.Domain.Fees;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Orders;

public sealed class OrderPayment : ITenantOwned, IAuditable, ISoftDeletable
{
    private OrderPayment()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public DateOnly PaidOn { get; private set; }
    public Guid? RecordedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedByUserId { get; private set; }
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
        Guid orderId, decimal amount, PaymentMethod method, DateOnly paidOn, Guid? recordedByUserId, DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            OrderId = orderId,
            Amount = amount,
            Method = method,
            PaidOn = paidOn,
            RecordedByUserId = recordedByUserId,
            CreatedAt = createdAt.ToUniversalTime(),
        };
}
