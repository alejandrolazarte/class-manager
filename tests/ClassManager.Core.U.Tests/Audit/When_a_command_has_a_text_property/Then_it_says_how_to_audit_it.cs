namespace ClassManager.Core.U.Tests.Audit.When_a_command_has_a_text_property;

public sealed class Then_it_says_how_to_audit_it
{
    [Fact]
    public void Then_it_says_how_to_audit_it_Run()
    {
        var unmarked = CommandProperties.All()
            .Where(CommandProperties.IsText)
            .Where(property => CommandProperties.AuditAttributesOf(property).Count != 1)
            .Select(CommandProperties.NameOf)
            .ToList();

        unmarked.ShouldBeEmpty();
    }
}
