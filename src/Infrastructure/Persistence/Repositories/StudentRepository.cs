using System.Linq.Expressions;


namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class StudentRepository(AppDbContext context) : IStudentRepository
{
    public void Add(Student student) => context.Students.Add(student);

    public Task<Student?> FindByClientAndNameAsync(Guid clientId, string fullName, CancellationToken cancellationToken) =>
        context.Students.AsNoTracking().FirstOrDefaultAsync(
            student => student.ClientId == clientId && student.FullName == fullName,
            cancellationToken);

    public async Task<IReadOnlyList<Student>> ListByClientAsync(Guid clientId, CancellationToken cancellationToken) =>
        await context.Students.AsNoTracking()
            .Where(student => student.ClientId == clientId)
            .OrderBy(student => student.FullName)
            .ToListAsync(cancellationToken);

    public Task<StudentSummary?> GetSummaryByIdAsync(Guid studentId, CancellationToken cancellationToken) =>
        StudentsWithClient()
            .Where(studentWithClient => studentWithClient.Student.Id == studentId)
            .Select(ToSummary)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<StudentSummary>> SearchAsync(StudentSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var studentsWithClient = StudentsWithClient();

        if (criteria.FullNameFragment is not null)
        {
            var fullNamePattern = LikePatterns.Contains(criteria.FullNameFragment);
            var phoneNumberPattern = LikePatterns.StartsWith(criteria.ClientPhoneNumberPrefix);

            studentsWithClient = studentsWithClient.Where(studentWithClient =>
                EF.Functions.Like(studentWithClient.Student.FullName, fullNamePattern, LikePatterns.EscapeCharacter)
                || EF.Functions.Like(studentWithClient.Client.FullName, fullNamePattern, LikePatterns.EscapeCharacter)
                || (phoneNumberPattern != null
                    && EF.Functions.Like((string)(object)studentWithClient.Client.PhoneNumber, phoneNumberPattern, LikePatterns.EscapeCharacter)));
        }

        return await studentsWithClient
            .OrderBy(studentWithClient => studentWithClient.Student.FullName)
            .Take(criteria.Limit)
            .Select(ToSummary)
            .ToListAsync(cancellationToken);
    }

    private static readonly Expression<Func<StudentWithClient, StudentSummary>> ToSummary =
        studentWithClient => new StudentSummary(
            studentWithClient.Student.Id,
            studentWithClient.Student.FullName,
            studentWithClient.Student.BirthDate,
            studentWithClient.Student.Notes,
            studentWithClient.Client.Id,
            studentWithClient.Client.FullName,
            studentWithClient.Client.PhoneNumber.Value);

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
