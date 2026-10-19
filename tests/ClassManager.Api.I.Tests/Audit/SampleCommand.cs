using ClassManager.Core.Audit;
using ClassManager.Core.UseCases;

namespace ClassManager.Api.I.Tests.Audit;

internal sealed record SampleCommand(
    Guid ClientId,
    [property: AuditPersonal] string FullName,
    decimal Amount) : ICommand;
