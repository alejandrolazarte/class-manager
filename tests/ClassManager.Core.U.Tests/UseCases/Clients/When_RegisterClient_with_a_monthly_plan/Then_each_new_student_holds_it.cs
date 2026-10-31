using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_a_monthly_plan;

public sealed class Then_each_new_student_holds_it
{
    [Fact]
    public async Task Then_each_new_student_holds_it_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var monthlyPlan = ClassPack.Create("Cuota mensual", null, 25000m, null, TestData.Now, renewsMonthly: true).Value!.WithId();
        builder.ClassPacks.Setup(repository => repository.GetByIdAsync(monthlyPlan.Id, It.IsAny<CancellationToken>())).ReturnsAsync(monthlyPlan);
        var command = RegisterClientUseCaseBuilder.CommandWithStudents("Tomás Pérez", "Lucía Pérez") with { PlanId = monthlyPlan.Id };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        builder.StudentPlans.Verify(repository => repository.Add(It.Is<StudentPlanChange>(change => change.ClassPackId == monthlyPlan.Id)), Times.Exactly(2));
        response.Value!.Plans.Students.ShouldAllBe(studentPlan => studentPlan.ClassPackId == monthlyPlan.Id);
    }
}
