-- Changes a brand's plan and price. Run it in the Azure portal (SQL database -> Query editor).
-- Fill in the five values below, then run the whole script.

DECLARE @OwnerEmail nvarchar(256) = N'owner@example.com';
DECLARE @PlanCode nvarchar(40) = N'enterprise';
DECLARE @Price decimal(18, 2) = 0;
DECLARE @Currency nchar(3) = N'EUR';
DECLARE @Note nvarchar(200) = N'Own business';

SET XACT_ABORT ON;

DECLARE @Brands TABLE (OrganizationId bigint);
INSERT INTO @Brands
SELECT DISTINCT member.OrganizationId
FROM OrganizationMembers member
JOIN [identity].[AspNetUsers] person ON person.Id = member.UserId
WHERE person.NormalizedEmail = UPPER(@OwnerEmail) AND member.Role = N'BrandOwner' AND member.IsDeleted = 0;

IF (SELECT COUNT(*) FROM @Brands) <> 1
    THROW 50000, 'That email must be the owner of exactly one brand.', 1;

IF NOT EXISTS (SELECT 1 FROM billing.Plans WHERE Code = @PlanCode)
    THROW 50001, 'There is no plan with that code.', 1;

DECLARE @OrganizationId bigint = (SELECT OrganizationId FROM @Brands);
DECLARE @Now datetimeoffset = SYSUTCDATETIME();

IF EXISTS (
    SELECT 1 FROM billing.Subscriptions
    WHERE SubscriberId = @OrganizationId AND IsDeleted = 0 AND ExternalSubscriptionId IS NOT NULL
      AND (ExpiredOn IS NULL OR ExpiredOn >= CAST(@Now AS date)))
    THROW 50002, 'The brand pays through Stripe: change its plan from Stripe.', 1;

BEGIN TRANSACTION;

UPDATE billing.Subscriptions
SET IsDeleted = 1, DeletedAt = @Now
WHERE SubscriberId = @OrganizationId AND IsDeleted = 0;

INSERT INTO billing.Subscriptions (SubscriberId, PlanCode, Price, Currency, Note, CreatedOn, IsDeleted, DeletedAt, DeletedByUserId, ExpiredOn)
VALUES (@OrganizationId, @PlanCode, @Price, UPPER(@Currency), NULLIF(LTRIM(RTRIM(@Note)), N''), @Now, 0, NULL, NULL, NULL);

COMMIT;

SELECT organization.Name, subscription.PlanCode, subscription.Price, subscription.Currency, subscription.Note,
       subscription.CreatedOn, subscription.DeletedAt
FROM billing.Subscriptions subscription
JOIN Organizations organization ON organization.Id = subscription.SubscriberId
WHERE subscription.SubscriberId = @OrganizationId
ORDER BY subscription.CreatedOn;
