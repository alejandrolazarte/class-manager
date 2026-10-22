namespace ClassManager.Core.U.Tests.UseCases.Students.When_AddStudent_checks_the_name;

public sealed class Then_the_people_of_the_business_are_locked_first
{
    [Fact]
    public async Task Then_the_people_of_the_business_are_locked_first_Run()
    {
        var builder = new AddStudentUseCaseBuilder();
        var steps = new List<string>();
        builder.PeopleLock.Setup(peopleLock => peopleLock.LockAsync(It.IsAny<CancellationToken>())).Callback(() => steps.Add(nameof(builder.PeopleLock)));
        builder.Students
            .Setup(repository => repository.FindByClientAndNameAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback(() => steps.Add(nameof(builder.Students)));

        await builder.Build().ExecuteAsync(AddStudentUseCaseBuilder.ValidCommand(), CancellationToken.None);

        steps.ShouldBe([nameof(builder.PeopleLock), nameof(builder.Students)]);
    }
}
