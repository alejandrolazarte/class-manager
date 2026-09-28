using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal static class ClientScopeQuery
{
    public static IQueryable<Guid> ClientIdsIn(AppDbContext context, ClientScope scope)
    {
        var registeredClientIds = context.Clients
            .Where(client => client.RegisteredByUserId == scope.RegisteredByUserId)
            .Select(client => client.Id);
        if (scope.InstructorId is not { } instructorId)
        {
            return registeredClientIds;
        }

        var enrolledClientIds =
            from enrollment in context.Enrollments
            join classGroup in context.ClassGroups on enrollment.ClassGroupId equals classGroup.Id
            join student in context.Students on enrollment.StudentId equals student.Id
            where classGroup.InstructorId == instructorId
            select student.ClientId;
        var privateLessonClientIds =
            from lessonStudent in context.PrivateLessonStudents
            join lesson in context.PrivateLessons on lessonStudent.PrivateLessonId equals lesson.Id
            join student in context.Students on lessonStudent.StudentId equals student.Id
            where lesson.InstructorId == instructorId
            select student.ClientId;

        return registeredClientIds.Concat(enrolledClientIds).Concat(privateLessonClientIds);
    }
}
