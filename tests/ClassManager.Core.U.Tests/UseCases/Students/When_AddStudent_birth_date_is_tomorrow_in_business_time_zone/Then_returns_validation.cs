using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_AddStudent_birth_date_is_tomorrow_in_business_time_zone;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new AddStudentUseCaseBuilder();
        var utcMidnightThatIsStillYesterdayInBuenosAires = new DateTimeOffset(2026, 9, 25, 1, 0, 0, TimeSpan.Zero);
        builder.TimeProvider.SetUtcNow(utcMidnightThatIsStillYesterdayInBuenosAires);
        var command = AddStudentUseCaseBuilder.ValidCommand() with { BirthDate = new DateOnly(2026, 9, 25) };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(Student.BirthDate));
    }
}
