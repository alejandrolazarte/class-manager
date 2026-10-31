using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Students;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Fees;

public sealed class StudentPlanChange : ITenantOwned, IAuditable, ISoftDeletable
{
    private const string PlanMustRenewMessage = "Choose a plan that renews every month.";
    private const string CustomPriceWithoutPlanMessage = "Only a plan has a price of its own.";
    private const decimal IncludedInTheFamilyFee = 0m;

    private StudentPlanChange()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long StudentId { get; private set; }
    public Student? Student { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public long? ClassPackId { get; private set; }
    public decimal? CustomPrice { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }

    public bool HasPlan => ClassPackId is not null;

    public static Result<StudentPlanChange> Create(
        long studentId,
        BillingMonth effectiveFrom,
        ClassPack? plan,
        decimal? customPrice,
        DateOnly today,
        DateTimeOffset createdAt)
    {
        if (!effectiveFrom.IsWithinAllowedRange(today))
        {
            return Result.Validation<StudentPlanChange>(BillingMonth.AllowedRangeMessage, fieldName: nameof(EffectiveFrom));
        }

        if (plan is { RenewsMonthly: false })
        {
            return Result.Validation<StudentPlanChange>(PlanMustRenewMessage, fieldName: nameof(ClassPackId));
        }

        if (plan is null && customPrice is not null)
        {
            return Result.Validation<StudentPlanChange>(CustomPriceWithoutPlanMessage, fieldName: nameof(CustomPrice));
        }

        if (customPrice is not null and not IncludedInTheFamilyFee)
        {
            var priceValidation = MonthlyFee.Validate(customPrice.Value, nameof(CustomPrice));
            if (priceValidation.IsFailure)
            {
                return priceValidation.Error!;
            }
        }

        return new StudentPlanChange
        {
            StudentId = studentId,
            EffectiveFrom = effectiveFrom.FirstDay,
            ClassPackId = plan?.Id,
            CustomPrice = customPrice,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public static Result<StudentPlanChange> ForStudent(
        Student student,
        BillingMonth effectiveFrom,
        ClassPack? plan,
        decimal? customPrice,
        DateOnly today,
        DateTimeOffset createdAt)
    {
        var change = Create(student.Id, effectiveFrom, plan, customPrice, today, createdAt);
        if (change.IsSuccess)
        {
            change.Value!.Student = student;
        }

        return change;
    }

    public void ReplaceWith(StudentPlanChange change)
    {
        ClassPackId = change.ClassPackId;
        CustomPrice = change.CustomPrice;
    }

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
