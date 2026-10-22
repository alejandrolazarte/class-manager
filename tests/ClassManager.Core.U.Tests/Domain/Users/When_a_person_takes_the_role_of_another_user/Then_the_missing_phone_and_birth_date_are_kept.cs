using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Users.When_a_person_takes_the_role_of_another_user;

public sealed class Then_the_missing_phone_and_birth_date_are_kept
{
    private static readonly DateOnly BirthDate = new(1990, 4, 12);

    [Fact]
    public void Then_the_missing_phone_and_birth_date_are_kept_Run()
    {
        var person = User.Create("Ana Gómez", "ana@example.com", TestData.Now).Value!.WithId();
        var previousPerson = Client.Create("Ana Gómez", PhoneNumber.Create("11 2233-4455", TestData.DefaultCountryCallingCode).Value!, null, null, TestData.Now).Value!.WithId().User;
        previousPerson.ChangeBirthDate(BirthDate, TestData.Today);

        person.CompleteWith(previousPerson);

        person.PhoneNumber.ShouldBe(previousPerson.PhoneNumber);
        person.BirthDate.ShouldBe(BirthDate);
    }
}
