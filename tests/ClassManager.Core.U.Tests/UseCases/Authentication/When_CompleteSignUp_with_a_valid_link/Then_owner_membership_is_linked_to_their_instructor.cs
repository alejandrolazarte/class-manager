namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_CompleteSignUp_with_a_valid_link;

public sealed class Then_owner_membership_is_linked_to_their_instructor
{
    [Fact]
    public async Task Then_owner_membership_is_linked_to_their_instructor_Run()
    {
        var builder = new CompleteSignUpUseCaseBuilder();

        await builder.Build().ExecuteAsync(CompleteSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.AddedMember!.InstructorId.ShouldBe(builder.AddedInstructor!.Id);
    }
}
