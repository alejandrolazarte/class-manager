namespace ClassManager.Core.UseCases.Students;

public sealed record NewStudent(
    string? FullName,
    DateOnly? BirthDate,
    string? Notes);
