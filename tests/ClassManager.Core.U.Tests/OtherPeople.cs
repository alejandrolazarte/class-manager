using ClassManager.Core.Abstractions.Persistence;

namespace ClassManager.Core.U.Tests;

internal static class OtherPeople
{
    public static void HasPersonWithTheirEmail(this Mock<IUserRepository> users, User person, PersonRoles roles)
    {
        users
            .Setup(repository => repository.FindByEmailForUpdateAsync(person.Email!, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);
        users.Setup(repository => repository.GetRolesAsync(person.Id, It.IsAny<CancellationToken>())).ReturnsAsync(roles);
    }
}
