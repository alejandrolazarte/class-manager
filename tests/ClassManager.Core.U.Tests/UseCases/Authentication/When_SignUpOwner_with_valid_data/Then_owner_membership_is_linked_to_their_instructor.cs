namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignUpOwner_with_valid_data;

public sealed class Then_owner_membership_is_linked_to_their_instructor
{
    [Fact]
    public async Task Then_owner_membership_is_linked_to_their_instructor_Run()
    {
        var builder = new SignUpOwnerUseCaseBuilder();

        await builder.Build().ExecuteAsync(SignUpOwnerUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.AddedMember!.InstructorId.ShouldBe(builder.AddedInstructor!.Id);
    }
}
