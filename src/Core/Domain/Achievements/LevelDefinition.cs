using ClassManager.Core.Audit;

namespace ClassManager.Core.Domain.Achievements;

public sealed record LevelDefinition([property: AuditPlain] string Name, int RequiredClasses);
