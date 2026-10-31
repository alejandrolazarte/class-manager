using System.Text;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Students;
using ClassManager.ImportExport;
using ClassManager.ImportExport.Parsing;
using ClassManager.ImportExport.Tabular.Csv;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport;

internal sealed class StudentImportBuilder
{
    public const string Header = "Alumno;Teléfono;Email;Fecha de nacimiento;Responsable\n";

    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IStudentRepository> Students { get; } = new();
    public Mock<IUserRepository> Users { get; } = new();
    public Mock<IFeeScheduleRepository> FeeSchedule { get; } = new();
    public List<DefaultMonthlyFeeChange> DefaultFeeChanges { get; } = [];
    public List<ClientBillingPlanChange> AddedPlanChanges { get; } = [];
    public List<Client> ExistingClients { get; } = [];
    public List<Student> ExistingStudents { get; } = [];
    public List<Client> AddedClients { get; } = [];
    public List<Student> AddedStudents { get; } = [];

    public StudentImportBuilder()
    {
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
        Users.Setup(repository => repository.ListEmailsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new HashSet<string>());
        Clients
            .Setup(repository => repository.ListByPhoneNumbersAsync(It.IsAny<IReadOnlyCollection<PhoneNumber>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<PhoneNumber> phoneNumbers, CancellationToken _) =>
                ExistingClients.Where(client => phoneNumbers.Contains(client.User.PhoneNumber)).ToList());
        Students
            .Setup(repository => repository.ListByClientIdsAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<long> clientIds, CancellationToken _) =>
                ExistingStudents.Where(student => clientIds.Contains(student.ClientId)).ToList());
        Clients.Setup(repository => repository.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => ExistingClients);
        Students.Setup(repository => repository.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => ExistingStudents);
        Clients.Setup(repository => repository.Add(It.IsAny<Client>())).Callback<Client>(AddedClients.Add);
        Students.Setup(repository => repository.Add(It.IsAny<Student>())).Callback<Student>(AddedStudents.Add);
        FeeSchedule.Setup(repository => repository.ListDefaultFeeChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => DefaultFeeChanges);
        FeeSchedule.Setup(repository => repository.Add(It.IsAny<ClientBillingPlanChange>())).Callback<ClientBillingPlanChange>(AddedPlanChanges.Add);
    }

    public StudentImportModule Module =>
        new(Businesses.Object, Clients.Object, Students.Object, Users.Object, FeeSchedule.Object, new FakeTimeProvider(TestData.Now));

    public Client AddExistingClient(params string[] studentNames)
    {
        var client = TestData.Client();
        ExistingClients.Add(client);
        foreach (var studentName in studentNames)
        {
            AddExistingStudent(client, studentName, null);
        }

        return client;
    }

    public Client AddExistingClient(string fullName, string phoneNumber, string? email, string? notes)
    {
        var client = Client.Create(fullName, PhoneNumber.Create(phoneNumber, TestData.DefaultCountryCallingCode).Value!, email, notes, TestData.Now).Value!.WithId();
        ExistingClients.Add(client);
        return client;
    }

    public void AddExistingStudent(Client client, string fullName, DateOnly? birthDate, string? notes = null, string? email = null) =>
        ExistingStudents.Add(TestUsers.StudentOf(client, fullName, birthDate, notes, email));

    public async Task<Result<ImportPlan>> PlanAsync(string rowsText, string header = Header)
    {
        var parser = new ImportParser(new CsvTabularReader());
        using var file = new MemoryStream(Encoding.UTF8.GetBytes(header + rowsText));
        var parsed = await parser.ParseAsync(file, Module.Columns, ImportLimits.Default, CancellationToken.None);
        return await Module.PlanAsync(parsed.Import!.Rows, CancellationToken.None);
    }

    public async Task<ImportPlan> PlanAndAddAsync(string rowsText, string header = Header)
    {
        var plan = (await PlanAsync(rowsText, header)).Value!;
        plan.AddValidRows();
        return plan;
    }
}
