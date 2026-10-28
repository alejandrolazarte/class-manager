-- Lists the families whose class pack balance can change when packs only pay the classes they cover
-- (docs/backend/20261028-class-pack-coverage/plan.md). Read-only. Run it in the Azure portal
-- (SQL database -> Query editor) BEFORE merging that change (merging to main deploys it), on the schema before its migration.
--
-- A row is a client with class pack sales and attended classes that none of their packs covers:
-- a group class outside every pack's class list, or a private lesson without a private lesson pack.
-- Today those classes are paid by any valid pack; after the change they are owed, or paid by another pack.
-- It is a review list, not the exact new balance: a class in a month the family paid with the monthly
-- fee shows up here too, although it never used a pack.

WITH ClientPurchases AS (
    SELECT purchase.TenantId, purchase.ClientId, purchase.Id AS PurchaseId, purchase.ClassPackId, purchase.ClassDurationMinutes
    FROM ClassPackPurchases AS purchase
    WHERE purchase.IsDeleted = 0
),
AttendedGroupClasses AS (
    SELECT student.TenantId, student.ClientId, session.ClassGroupId, session.Date
    FROM Attendances AS attendance
    JOIN Students AS student ON student.Id = attendance.StudentId
    JOIN ClassSessions AS session ON session.Id = attendance.ClassSessionId
    WHERE attendance.Status = N'Present'
       OR EXISTS (SELECT 1 FROM PackBookings AS booking
                  WHERE booking.ClassSessionId = attendance.ClassSessionId AND booking.StudentId = attendance.StudentId)
),
UncoveredGroupClasses AS (
    SELECT attended.TenantId, attended.ClientId, COUNT(*) AS Classes
    FROM AttendedGroupClasses AS attended
    WHERE EXISTS (SELECT 1 FROM ClientPurchases AS purchase WHERE purchase.ClientId = attended.ClientId)
      AND NOT EXISTS (
          SELECT 1
          FROM ClientPurchases AS purchase
          JOIN ClassPackClassGroups AS packClassGroup ON packClassGroup.ClassPackId = purchase.ClassPackId
          WHERE purchase.ClientId = attended.ClientId AND packClassGroup.ClassGroupId = attended.ClassGroupId)
    GROUP BY attended.TenantId, attended.ClientId
),
UncoveredPrivateLessons AS (
    SELECT student.TenantId, student.ClientId, COUNT(*) AS Classes
    FROM PrivateLessonStudents AS lessonStudent
    JOIN PrivateLessons AS lesson ON lesson.Id = lessonStudent.PrivateLessonId
    JOIN Students AS student ON student.Id = lessonStudent.StudentId
    WHERE lessonStudent.Status = N'Present'
      AND lesson.IsTrial = 0
      AND EXISTS (SELECT 1 FROM ClientPurchases AS purchase WHERE purchase.ClientId = student.ClientId)
      AND NOT EXISTS (
          SELECT 1 FROM ClientPurchases AS purchase
          WHERE purchase.ClientId = student.ClientId AND purchase.ClassDurationMinutes IS NOT NULL)
    GROUP BY student.TenantId, student.ClientId
)
SELECT
    business.Name AS Business,
    person.FullName AS Client,
    COALESCE(groupClasses.Classes, 0) AS GroupClassesNoPackCovers,
    COALESCE(privateLessons.Classes, 0) AS PrivateLessonsNoPackCovers
FROM (
    SELECT TenantId, ClientId FROM UncoveredGroupClasses
    UNION
    SELECT TenantId, ClientId FROM UncoveredPrivateLessons
) AS affected
JOIN Businesses AS business ON business.Id = affected.TenantId
JOIN Clients AS client ON client.Id = affected.ClientId
JOIN Users AS person ON person.Id = client.UserId
LEFT JOIN UncoveredGroupClasses AS groupClasses ON groupClasses.ClientId = affected.ClientId
LEFT JOIN UncoveredPrivateLessons AS privateLessons ON privateLessons.ClientId = affected.ClientId
ORDER BY business.Name, person.FullName;
