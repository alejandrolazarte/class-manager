-- Lists the emails used by more than one person in the same business.
-- Run it in the Azure portal (SQL database -> Query editor) before deploying the migration RemovePeopleCopies,
-- which fails while any is left. Each row is one person with a repeated email and where they show up in the app:
-- fix them there (the same person loaded twice, or two people sharing an email) and run it again until it is empty.

SELECT
    business.Name AS Business,
    person.Email,
    person.FullName,
    CASE
        WHEN EXISTS (SELECT 1 FROM Clients client WHERE client.UserId = person.Id) THEN N'Client'
        WHEN EXISTS (SELECT 1 FROM Students student WHERE student.UserId = person.Id) THEN N'Student'
        WHEN EXISTS (SELECT 1 FROM Instructors instructor WHERE instructor.UserId = person.Id) THEN N'Instructor'
        WHEN EXISTS (SELECT 1 FROM BusinessMembers member WHERE member.UserId = person.Id AND member.IsDeleted = 0) THEN N'Team'
        ELSE N'Brand owner'
    END AS ShowsUpAs,
    person.Id AS UserId
FROM Users person
JOIN Businesses business ON business.Id = person.TenantId
WHERE person.IsDeleted = 0
    AND person.Email IS NOT NULL
    AND EXISTS (
        SELECT 1 FROM Users other
        WHERE other.TenantId = person.TenantId AND other.Email = person.Email AND other.Id <> person.Id AND other.IsDeleted = 0)
ORDER BY business.Name, person.Email, person.FullName;
