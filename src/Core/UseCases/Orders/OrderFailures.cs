using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

internal static class OrderFailures
{
    private const string NotFoundMessage = "The order does not exist.";
    private const string LineNotFoundMessage = "The item is not part of this order.";
    private const string LineItemMessage = "Each item must be either a class pack or a product size.";

    public static ResultError NotFound() => new(OrderErrorCodes.NotFound, NotFoundMessage, ErrorKind.NotFound);

    public static ResultError LineNotFound() =>
        new(OrderErrorCodes.LineNotFound, LineNotFoundMessage, ErrorKind.Validation) { FieldName = OrderLine.LinesFieldName };

    public static ResultError LineItemRequired() =>
        new(Result.ValidationCode, LineItemMessage, ErrorKind.Validation) { FieldName = OrderLine.LinesFieldName };
}
