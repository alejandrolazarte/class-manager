using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests;

internal static class TestUsers
{
    public static Student StudentOf(Client client, string fullName, DateOnly? birthDate = null, string? notes = null, string? email = null) =>
        AttendingContact.NamesTheContact(client.User.FullName, fullName)
            ? Student.CreateAttendingContact(client, birthDate, notes, TestData.Today, TestData.Now).Value!
            : Student.Create(client.Id, fullName, birthDate, notes, TestData.Today, TestData.Now, email).Value!;
}
