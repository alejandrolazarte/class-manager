using ClassManager.Core.Audit;
using ClassManager.Core.UseCases;

namespace ClassManager.Api.I.Tests.Audit;

internal sealed record SampleQuery([property: AuditPlain] string Module) : IQuery;
