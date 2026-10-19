using System.Text.RegularExpressions;
using ClassManager.Core.Audit;

namespace ClassManager.Core.U.Tests.Audit.When_a_command_property_is_named_like_personal_data;

public sealed partial class Then_it_is_marked_personal
{
    [Fact]
    public void Then_it_is_marked_personal_Run()
    {
        var notPersonal = CommandProperties.All()
            .Where(property => PersonalName().IsMatch(property.Name))
            .Where(property => !CommandProperties.AuditAttributesOf(property).SequenceEqual([typeof(AuditPersonalAttribute)]))
            .Select(CommandProperties.NameOf)
            .ToList();

        notPersonal.ShouldBeEmpty();
    }

    [GeneratedRegex("FullName|Email|Phone|Birth|Address|Note|Comment|Reason|^Text$|^Body$|^Value$")]
    private static partial Regex PersonalName();
}
