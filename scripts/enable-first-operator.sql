-- Enables the first backoffice operator, before any operator can do it from the backoffice.
-- Run it in the Azure portal (SQL database -> Query editor), see docs/backoffice-access.md.
-- 1. The person signs in to the backoffice with Google once and gets "Acceso no permitido" with a code.
-- 2. Fill in the values below and run the whole script. @BranchName is needed only when the account is a
--    team member of more than one Laztty branch: the platform role goes to that branch, the others keep theirs.

DECLARE @AccessRequestCode nvarchar(6) = N'K7-3QF';
DECLARE @AccountEmail nvarchar(256) = N'owner@example.com';
DECLARE @PlatformOrganizationId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @BranchName nvarchar(120) = NULL;
DECLARE @Role nvarchar(20) = N'PlatformOperator';

SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;

IF @Role NOT IN (N'PlatformOperator', N'PlatformReader')
    THROW 50000, 'The role must be PlatformOperator or PlatformReader.', 1;

DECLARE @AccessRequestId uniqueidentifier;
DECLARE @Provider nvarchar(40);
DECLARE @ProviderKey nvarchar(255);
DECLARE @ProviderEmail nvarchar(256);
SELECT @AccessRequestId = Id, @Provider = Provider, @ProviderKey = ProviderKey, @ProviderEmail = Email
FROM backoffice.BackofficeAccessRequests
WHERE Code = UPPER(@AccessRequestCode);

IF @AccessRequestId IS NULL
    THROW 50001, 'There is no access request with that code.', 1;

DECLARE @AccountId uniqueidentifier = (
    SELECT Id FROM [identity].[AspNetUsers] WHERE NormalizedEmail = UPPER(@AccountEmail));

IF @AccountId IS NULL
    THROW 50002, 'There is no account with that email.', 1;

DECLARE @Members TABLE (MemberId uniqueidentifier);
INSERT INTO @Members
SELECT member.Id
FROM BusinessMembers member
JOIN Users person ON person.Id = member.UserId
JOIN Businesses business ON business.Id = member.TenantId
WHERE person.AccountId = @AccountId AND business.OrganizationId = @PlatformOrganizationId AND member.IsDeleted = 0
  AND (@BranchName IS NULL OR business.Name = @BranchName);

IF (SELECT COUNT(*) FROM @Members) <> 1
    THROW 50003, 'That account must be a team member of exactly one branch of the platform organization; with more than one, set @BranchName.', 1;

DECLARE @Now datetimeoffset = SYSUTCDATETIME();

BEGIN TRANSACTION;

UPDATE BusinessMembers
SET Role = @Role, CustomRoleId = NULL, InstructorId = NULL, UpdatedAt = @Now
WHERE Id = (SELECT MemberId FROM @Members);

INSERT INTO backoffice.BackofficeLogins (Id, Provider, ProviderKey, AccountId, Email, EnabledAt, EnabledByUserId)
VALUES (NEWID(), @Provider, @ProviderKey, @AccountId, @ProviderEmail, @Now, NULL);

DELETE FROM backoffice.BackofficeAccessRequests WHERE Id = @AccessRequestId;

COMMIT;

SELECT login.Provider, login.Email, login.EnabledAt, member.Role, business.Name AS Branch
FROM backoffice.BackofficeLogins login
JOIN Users person ON person.AccountId = login.AccountId
JOIN BusinessMembers member ON member.UserId = person.Id AND member.IsDeleted = 0
JOIN Businesses business ON business.Id = member.TenantId
WHERE login.AccountId = @AccountId AND business.OrganizationId = @PlatformOrganizationId;
