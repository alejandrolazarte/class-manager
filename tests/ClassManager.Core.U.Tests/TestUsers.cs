using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.Domain.Users;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests;

internal static class TestUsers
{
    public static Client WithUser(this Client client)
    {
        var user = NewUser(client.FullName);
        user.ChangeEmail(client.Email);
        user.ChangePhoneNumber(client.PhoneNumber);
        client.LinkUser(user);
        return client;
    }

    public static Student WithUser(this Student student, Client client)
    {
        if (AttendingContact.IsTheContact(client.FullName, student.FullName))
        {
            student.LinkUser(client.User);
        }
        else
        {
            var user = NewUser(student.FullName);
            user.ChangeEmail(student.Email);
            student.LinkUser(user);
        }

        student.User.ChangeBirthDate(student.BirthDate);
        return student;
    }

    public static Instructor WithUser(this Instructor instructor)
    {
        var user = NewUser(instructor.FullName);
        user.ChangeEmail(instructor.Email);
        instructor.LinkUser(user);
        return instructor;
    }

    private static User NewUser(string fullName) => User.Create(Guid.CreateVersion7(), fullName, TestData.Now);
}
