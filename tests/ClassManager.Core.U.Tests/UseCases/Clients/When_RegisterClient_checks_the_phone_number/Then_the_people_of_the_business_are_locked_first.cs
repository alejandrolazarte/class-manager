using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_checks_the_phone_number;

public sealed class Then_the_people_of_the_business_are_locked_first
{
    [Fact]
    public async Task Then_the_people_of_the_business_are_locked_first_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var steps = new List<string>();
        builder.PeopleLock.Setup(peopleLock => peopleLock.LockAsync(It.IsAny<CancellationToken>())).Callback(() => steps.Add(nameof(builder.PeopleLock)));
        builder.Clients
            .Setup(repository => repository.FindByPhoneNumberAsync(It.IsAny<PhoneNumber>(), It.IsAny<CancellationToken>()))
            .Callback(() => steps.Add(nameof(builder.Clients)));

        await builder.Build().ExecuteAsync(RegisterClientUseCaseBuilder.ValidCommand(), CancellationToken.None);

        steps.ShouldBe([nameof(builder.PeopleLock), nameof(builder.Clients)]);
    }
}
