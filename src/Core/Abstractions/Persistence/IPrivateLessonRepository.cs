using ClassManager.Core.Domain.PrivateLessons;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IPrivateLessonRepository
{
    void Add(PrivateLesson lesson);

    void Remove(PrivateLesson lesson);

    Task<PrivateLesson?> GetByIdAsync(long privateLessonId, CancellationToken cancellationToken);

    Task<PrivateLesson?> GetForUpdateAsync(long privateLessonId, CancellationToken cancellationToken);

    Task<IReadOnlyList<PrivateLesson>> ListBetweenAsync(DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<PrivateLesson>> ListByInstructorOnDatesAsync(
        long instructorId,
        IReadOnlyCollection<DateOnly> dates,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PrivateLesson>> ListByInstructorFromAsync(long instructorId, DateOnly firstDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<PaidTrialLesson>> ListPaidTrialsByClientAsync(long clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientAttendedClass>> ListAttendedClassesByClientsAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken);
}
