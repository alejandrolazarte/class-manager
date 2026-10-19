using System.Text.RegularExpressions;
using ClassManager.Core.Audit;

namespace ClassManager.Core.U.Tests.Audit.When_a_command_property_is_named_like_a_secret;

public sealed partial class Then_it_is_marked_secret
{
    [Fact]
    public void Then_it_is_marked_secret_Run()
    {
        var notSecret = CommandProperties.All()
            .Where(property => SecretName().IsMatch(property.Name))
            .Where(property => !CommandProperties.AuditAttributesOf(property).SequenceEqual([typeof(AuditSecretAttribute)]))
            .Select(CommandProperties.NameOf)
            .ToList();

        notSecret.ShouldBeEmpty();
    }

    [GeneratedRegex("Password|Token|Secret|Key|^Endpoint$|^Auth$|^P256dh$")]
    private static partial Regex SecretName();
}
