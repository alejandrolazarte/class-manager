using ClassManager.Core.Audit;
using ClassManager.Core.UseCases;

namespace ClassManager.Api.I.Tests.Audit;

[AuditedQuery]
internal sealed record SampleAuditedQuery([property: AuditPlain] string Module) : IQuery;
