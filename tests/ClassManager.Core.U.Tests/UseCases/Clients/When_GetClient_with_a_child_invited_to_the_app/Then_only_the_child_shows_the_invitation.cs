using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_GetClient_with_a_child_invited_to_the_app;

public sealed class Then_only_the_child_shows_the_invitation
{
    [Fact]
    public async Task Then_only_the_child_shows_the_invitation_Run()
    {
        var builder = new GetClientUseCaseBuilder();
        var child = Student.Create(builder.Client.Id, TestData.StudentFullName, new DateOnly(2012, 5, 1), null, TestData.Today, TestData.Now, "tomas@example.com").Value!;
        builder.Students.Setup(repository => repository.ListByClientAsync(builder.Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync([child]);
        builder.Invitations
            .Setup(repository => repository.ListPendingByClientAsync(builder.Client.Id, TestData.Now, It.IsAny<CancellationToken>()))
            .ReturnsAsync([ClientInvitation.Create(builder.Client.Id, "tomas@example.com", "token-hash", Guid.CreateVersion7(), TestData.Now, child.Id).Value!]);

        var response = await builder.Build().ExecuteAsync(new GetClientQuery(builder.Client.Id), CancellationToken.None);

        response.Value!.Students.Single().AppAccess.ShouldBe(new StudentAppAccessResponse(StudentAppAccessStatus.Invited, "tomas@example.com"));
        response.Value.AppAccess.ShouldBe(StudentAppAccessResponse.NotInvited);
    }
}
