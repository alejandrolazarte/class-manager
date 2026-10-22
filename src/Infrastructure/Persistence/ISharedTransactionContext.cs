using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ClassManager.Infrastructure.Persistence;

public interface ISharedTransactionContext
{
    DatabaseFacade Database { get; }
}
