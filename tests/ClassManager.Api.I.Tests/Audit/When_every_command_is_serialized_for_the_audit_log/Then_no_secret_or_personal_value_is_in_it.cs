using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_every_command_is_serialized_for_the_audit_log;

public sealed class Then_no_secret_or_personal_value_is_in_it
{
    [Fact]
    public void Then_no_secret_or_personal_value_is_in_it_Run()
    {
        var leaks = new List<string>();
        foreach (var commandType in CommandSamples.CommandTypes())
        {
            var samples = new CommandSamples();
            var payload = CommandAuditPayload.Serialize(samples.Build(commandType)).Json;
            leaks.AddRange(samples.HiddenValues
                .Where(hiddenValue => payload.Contains(hiddenValue, StringComparison.Ordinal))
                .Select(hiddenValue => $"{commandType.Name}: {hiddenValue}"));
        }

        leaks.ShouldBeEmpty();
    }
}
