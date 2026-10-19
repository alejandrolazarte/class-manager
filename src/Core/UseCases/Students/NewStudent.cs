using ClassManager.Core.Audit;

namespace ClassManager.Core.UseCases.Students;

public sealed record NewStudent(
    [property: AuditPersonal] string? FullName,
    [property: AuditPersonal] DateOnly? BirthDate,
    [property: AuditPersonal] string? Notes,
    [property: AuditPersonal] string? Email = null,
    Guid? SamePersonUserId = null);
