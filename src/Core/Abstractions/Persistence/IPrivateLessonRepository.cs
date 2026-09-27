using ClassManager.Core.Domain.PrivateLessons;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IPrivateLessonRepository
{
    void Add(PrivateLesson lesson);

    void Remove(PrivateLesson lesson);

    Task<PrivateLesson?> GetByIdAsync(Guid privateLessonId, CancellationToken cancellationToken);

    Task<PrivateLesson?> GetForUpdateAsync(Guid privateLessonId, CancellationToken cancellationToken);

    Task<IReadOnlyList<PrivateLesson>> ListBetweenAsync(DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<PrivateLesson>> ListByInstructorOnDatesAsync(
        Guid instructorId,
        IReadOnlyCollection<DateOnly> dates,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PrivateLesson>> ListByInstructorFromAsync(Guid instructorId, DateOnly firstDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<PaidTrialLesson>> ListPaidTrialsByClientAsync(Guid clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientAttendedClass>> ListAttendedClassesByClientsAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken);
}
