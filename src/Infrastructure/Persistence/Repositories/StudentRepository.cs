using System.Linq.Expressions;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class StudentRepository(AppDbContext context) : IStudentRepository
{
    public void Add(Student student) => context.Students.Add(student);

    public Task<Student?> FindForUpdateAsync(Guid studentId, CancellationToken cancellationToken) =>
        context.Students.Include(student => student.User).FirstOrDefaultAsync(student => student.Id == studentId, cancellationToken);

    public Task<Student?> FindByClientAndNameAsync(Guid clientId, string fullName, CancellationToken cancellationToken) =>
        context.Students.AsNoTracking().FirstOrDefaultAsync(
            student => student.ClientId == clientId && student.User.FullName == fullName,
            cancellationToken);

    public async Task<IReadOnlyList<Student>> ListByClientAsync(Guid clientId, CancellationToken cancellationToken) =>
        await context.Students.AsNoTracking().Include(student => student.User)
            .Where(student => student.ClientId == clientId)
            .OrderBy(student => student.User.FullName)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Student>> ListByClientForUpdateAsync(Guid clientId, CancellationToken cancellationToken) =>
        await context.Students.Include(student => student.User)
            .Where(student => student.ClientId == clientId)
            .OrderBy(student => student.User.FullName)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Student>> ListAllAsync(CancellationToken cancellationToken) =>
        await context.Students.AsNoTracking().Include(student => student.User)
            .OrderBy(student => student.User.FullName)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Student>> ListByClientIdsAsync(
        IReadOnlyCollection<Guid> clientIds,
        CancellationToken cancellationToken) =>
        clientIds.Count == 0
            ? []
            : await context.Students.AsNoTracking().Include(student => student.User)
                .Where(student => clientIds.Contains(student.ClientId))
                .ToListAsync(cancellationToken);

    public Task<StudentSummary?> GetSummaryByIdAsync(Guid studentId, CancellationToken cancellationToken) =>
        StudentsWithClient()
            .Where(studentWithClient => studentWithClient.Student.Id == studentId)
            .Select(ToSummary)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<StudentSummary>> ListSummariesByIdsAsync(
        IReadOnlyCollection<Guid> studentIds,
        CancellationToken cancellationToken) =>
        studentIds.Count == 0
            ? []
            : await StudentsWithClient()
                .Where(studentWithClient => studentIds.Contains(studentWithClient.Student.Id))
                .Select(ToSummary)
                .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StudentSummary>> SearchAsync(StudentSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var studentsWithClient = StudentsWithClient();
        if (criteria.Scope is not null)
        {
            var clientIdsInScope = ClientScopeQuery.ClientIdsIn(context, criteria.Scope);
            studentsWithClient = studentsWithClient.Where(studentWithClient => clientIdsInScope.Contains(studentWithClient.Client.Id));
        }

        if (criteria.FullNameFragment is not null)
        {
            var fullNamePattern = LikePatterns.Contains(criteria.FullNameFragment);
            var phoneNumberPattern = LikePatterns.StartsWith(criteria.ClientPhoneNumberPrefix);

            studentsWithClient = studentsWithClient.Where(studentWithClient =>
                EF.Functions.Like(studentWithClient.Student.User.FullName, fullNamePattern, LikePatterns.EscapeCharacter)
                || EF.Functions.Like(studentWithClient.Client.User.FullName, fullNamePattern, LikePatterns.EscapeCharacter)
                || (phoneNumberPattern != null
                    && EF.Functions.Like((string)(object)studentWithClient.Client.User.PhoneNumber!, phoneNumberPattern, LikePatterns.EscapeCharacter)));
        }

        return await studentsWithClient
            .OrderBy(studentWithClient => studentWithClient.Student.User.FullName)
            .Take(criteria.Limit)
            .Select(ToSummary)
            .ToListAsync(cancellationToken);
    }

    private static readonly Expression<Func<StudentWithClient, StudentSummary>> ToSummary =
        studentWithClient => new StudentSummary(
            studentWithClient.Student.Id,
            studentWithClient.Student.User.FullName,
            studentWithClient.Student.User.BirthDate,
            studentWithClient.Student.Notes,
            studentWithClient.Client.Id,
            studentWithClient.Client.User.FullName,
            studentWithClient.Client.User.PhoneNumber!.Value);

    private IQueryable<StudentWithClient> StudentsWithClient() =>
        context.Students.AsNoTracking().Join(
            context.Clients.AsNoTracking(),
            student => student.ClientId,
            client => client.Id,
            (student, client) => new StudentWithClient { Student = student, Client = client });

    private sealed class StudentWithClient
    {
        public required Student Student { get; init; }
        public required Client Client { get; init; }
    }
}
