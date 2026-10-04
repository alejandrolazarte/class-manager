using System.Globalization;
using ClassManager.Core.Abstractions.Email;
using ClassManager.Notifications.Email;

namespace ClassManager.Infrastructure.Notifications;

internal static class OrderEmails
{
    public const string NumberPrefix = "n.º ";
    public const string PaidSubject = "Recibimos tu pago";
    public const string ReadySubject = "Tu pedido está listo";
    public const string CancelledSubject = "Se canceló tu pedido";

    public const string ClassesCredited = "Las clases ya están cargadas en tu cuenta.";
    public const string WillNotifyWhenReady = "Te avisamos cuando los productos estén listos para entregar.";

    private const string StudentAppEyebrowPrefix = "Pedido ";
    private const string TeamEyebrowPrefix = "Nuevo pedido · ";
    private const string DeliveryLabel = "Entrega";
    private const string StudentAppAction = "Ver pedido en la app";
    private const string TeamAction = "Abrir pedidos";
    private const string StudentAppFooter = "Recibís este mail porque hiciste un pedido en la app.";
    private const string AmountFormat = "N2";
    private const string QuantitySeparator = " × ";

    private static readonly CultureInfo SpanishCulture = CultureInfo.GetCultureInfo("es-ES");

    public static EmailMessage Placed(OrderEmailContext order, string clientName, string? delivery, string teamOrdersLink) =>
        new(
            order.To,
            PlacedSubject(order.Number, clientName),
            new EmailContent(
                TeamEyebrowPrefix + NumberOf(order.Number),
                $"{clientName} hizo un pedido desde la app",
                "Se paga en la sede. Cuando lo cobres, confirmá el pago en Pedidos.",
                $"Recibís este mail porque gestionás los pedidos de {order.BusinessName}.")
            {
                Lines = order.Lines,
                Total = order.Total,
                Note = delivery is null ? null : new EmailNote(delivery, DeliveryLabel),
                Action = new EmailAction(TeamAction, teamOrdersLink),
            },
            order.BusinessId);

    public static EmailMessage Paid(OrderEmailContext order, string nextStep, bool hasProducts, string studentAppOrdersLink) =>
        ForStudentApp(
            order,
            PaidSubject,
            $"{order.BusinessName} confirmó el pago de tu pedido.",
            new EmailNote(nextStep, hasProducts ? DeliveryLabel : null),
            studentAppOrdersLink);

    public static EmailMessage Ready(OrderEmailContext order, string whereToGetIt, string studentAppOrdersLink) =>
        ForStudentApp(
            order,
            ReadySubject,
            $"Tu pedido de {order.BusinessName} ya está listo para entregar.",
            new EmailNote(whereToGetIt, DeliveryLabel),
            studentAppOrdersLink);

    public static EmailMessage CancelledUnpaid(OrderEmailContext order, string studentAppOrdersLink) =>
        ForStudentApp(
            order,
            CancelledSubject,
            $"Tu pedido de {order.BusinessName} se canceló porque no se pagó en 7 días.",
            new EmailNote("Si todavía lo querés, podés pedirlo de nuevo desde la app."),
            studentAppOrdersLink);

    public static EmailMessage CancelledByBranch(OrderEmailContext order, string studentAppOrdersLink) =>
        ForStudentApp(
            order,
            CancelledSubject,
            $"{order.BusinessName} canceló tu pedido.",
            new EmailNote("Si tenés dudas, consultá en la sede."),
            studentAppOrdersLink);

    public static string PlacedSubject(int orderNumber, string clientName) => $"Nuevo pedido {NumberOf(orderNumber)} de {clientName}";

    public static string NumberOf(int orderNumber) => NumberPrefix + orderNumber.ToString(CultureInfo.InvariantCulture);

    public static EmailLine Line(int quantity, string name, decimal total, string currencyCode) =>
        new(quantity > 1 ? quantity + QuantitySeparator + name : name, Amount(total, currencyCode));

    public static string Amount(decimal amount, string currencyCode) =>
        $"{amount.ToString(AmountFormat, SpanishCulture)} {currencyCode}";

    public static string PickupAt(string businessName) => $"Podés retirarlo en {businessName}.";

    public static string InClass(string classGroupName) => $"Te lo entregan en la clase {classGroupName}.";

    public static string DeliveryForBranch(string? classGroupName) =>
        classGroupName is null ? "Retiro en la sede." : $"En la clase {classGroupName}.";

    private static EmailMessage ForStudentApp(OrderEmailContext order, string subject, string intro, EmailNote note, string studentAppOrdersLink) =>
        new(
            order.To,
            subject,
            new EmailContent(StudentAppEyebrowPrefix + NumberOf(order.Number), subject, intro, StudentAppFooter)
            {
                Lines = order.Lines,
                Total = order.Total,
                Note = note,
                Action = new EmailAction(StudentAppAction, studentAppOrdersLink),
            },
            order.BusinessId);
}

internal sealed record OrderEmailContext(string To, int Number, Guid BusinessId, string BusinessName, IReadOnlyList<EmailLine> Lines, string Total);
