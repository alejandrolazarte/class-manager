namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignUpOwner_with_valid_data;

public sealed class Then_the_organization_subscribes_to_the_sign_up_plan
{
    [Fact]
    public async Task Then_the_organization_subscribes_to_the_sign_up_plan_Run()
    {
        var builder = new SignUpOwnerUseCaseBuilder();

        await builder.Build().ExecuteAsync(SignUpOwnerUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.AddedSubscription!.SubscriberId.ShouldBe(builder.AddedOrganization!.Id);
        builder.AddedSubscription.PlanCode.ShouldBe(builder.SignUpPlan.Code);
        builder.AddedSubscription.Price.ShouldBe(0m);
    }
}
