using Microsoft.Data.SqlClient;

namespace ClassManager.Infrastructure.Audit;

internal static class ExceptionErrorCode
{
    private const string SqlErrorPrefix = "sql.";

    public static string? Of(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is SqlException sqlException)
            {
                return SqlErrorPrefix + sqlException.Number;
            }
        }

        return null;
    }
}
