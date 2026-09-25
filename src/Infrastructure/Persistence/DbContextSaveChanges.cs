using Microsoft.Data.SqlClient;

namespace ClassManager.Infrastructure.Persistence;

internal static class DbContextSaveChanges
{
    private const int UniqueIndexViolationErrorNumber = 2601;
    private const int UniqueConstraintViolationErrorNumber = 2627;

    public static async Task SaveChangesTranslatingConflictsAsync(this DbContext context, CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyConflictException(exception.Message, exception);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            throw new UniqueConstraintViolationException(exception.Message, exception);
        }
    }

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: UniqueIndexViolationErrorNumber or UniqueConstraintViolationErrorNumber };
}
