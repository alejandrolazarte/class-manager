using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.StudentApp;

internal static class StudentAppFailures
{
    private const string NoAccessMessage = "This account is not linked to a client.";
    private const string NoAccessCode = "student.no_access";
    private const string OrderNotFoundMessage = "The order does not exist.";
    private const string TooManyOpenMessage = "You already have 5 orders waiting for payment. Pay or cancel one first.";

    public static ResultError NoAccess() => new(NoAccessCode, NoAccessMessage, ErrorKind.Unauthorized);

    public static ResultError OrderNotFound() => new(OrderErrorCodes.NotFound, OrderNotFoundMessage, ErrorKind.NotFound);

    public static ResultError TooManyOpenOrders() => new(OrderErrorCodes.TooManyOpen, TooManyOpenMessage, ErrorKind.Conflict);
}
