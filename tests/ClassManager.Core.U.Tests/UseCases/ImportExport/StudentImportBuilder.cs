using System.Text;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Clients;
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
    public List<Client> ExistingClients { get; } = [];
    public List<Student> ExistingStudents { get; } = [];
    public List<Client> AddedClients { get; } = [];
    public List<Student> AddedStudents { get; } = [];

    public StudentImportBuilder()
    {
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
        Clients
            .Setup(repository => repository.ListByPhoneNumbersAsync(It.IsAny<IReadOnlyCollection<PhoneNumber>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<PhoneNumber> phoneNumbers, CancellationToken _) =>
                ExistingClients.Where(client => phoneNumbers.Contains(client.PhoneNumber)).ToList());
        Students
            .Setup(repository => repository.ListByClientIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Guid> clientIds, CancellationToken _) =>
                ExistingStudents.Where(student => clientIds.Contains(student.ClientId)).ToList());
        Clients.Setup(repository => repository.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => ExistingClients);
        Students.Setup(repository => repository.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => ExistingStudents);
        Clients.Setup(repository => repository.Add(It.IsAny<Client>())).Callback<Client>(AddedClients.Add);
        Students.Setup(repository => repository.Add(It.IsAny<Student>())).Callback<Student>(AddedStudents.Add);
    }

    public StudentImportModule Module =>
        new(Businesses.Object, Clients.Object, Students.Object, new FakeTimeProvider(TestData.Now));

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
        var client = Client.Create(fullName, PhoneNumber.Create(phoneNumber, TestData.DefaultCountryCallingCode).Value!, email, notes, TestData.Now).Value!;
        ExistingClients.Add(client);
        return client;
    }

    public void AddExistingStudent(Client client, string fullName, DateOnly? birthDate, string? notes = null, string? email = null) =>
        ExistingStudents.Add(Student.Create(client.Id, fullName, birthDate, notes, TestData.Today, TestData.Now, email).Value!);

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
