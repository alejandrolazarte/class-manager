using System.Globalization;
using ClassManager.Core.Abstractions.Email;

namespace ClassManager.Infrastructure.Notifications;

internal static class OrderEmails
{
    public const string PlacedSubjectPrefix = "Nuevo pedido de ";
    public const string PaidSubject = "Recibimos tu pago";
    public const string ReadySubject = "Tu pedido está listo";
    public const string CancelledSubject = "Se canceló tu pedido";

    private const string AmountFormat = "N2";
    private static readonly CultureInfo SpanishCulture = CultureInfo.GetCultureInfo("es-ES");

    public static EmailMessage Placed(string to, string familyName, string lines, string total, string delivery) =>
        new(
            to,
            PlacedSubjectPrefix + familyName,
            $"{familyName} hizo un pedido desde la app:\n\n{lines}\n\nTotal: {total}\n{delivery}\n\n" +
            "Se paga en la sede. Confirmá el pago en Ajustes → Pedidos.");

    public static EmailMessage Paid(string to, string businessName, string lines, string nextStep) =>
        new(to, PaidSubject, $"Hola,\n\n{businessName} confirmó el pago de tu pedido:\n\n{lines}\n\n{nextStep}");

    public static EmailMessage Ready(string to, string businessName, string lines, string whereToGetIt) =>
        new(to, ReadySubject, $"Hola,\n\nTu pedido de {businessName} está listo:\n\n{lines}\n\n{whereToGetIt}");

    public static EmailMessage CancelledUnpaid(string to, string businessName, string lines) =>
        new(
            to,
            CancelledSubject,
            $"Hola,\n\nTu pedido de {businessName} se canceló porque no se pagó en 7 días:\n\n{lines}\n\n" +
            "Si todavía lo querés, podés pedirlo de nuevo desde la app.");

    public static EmailMessage CancelledByBranch(string to, string businessName, string lines) =>
        new(
            to,
            CancelledSubject,
            $"Hola,\n\n{businessName} canceló tu pedido:\n\n{lines}\n\nSi tenés dudas, consultá en la sede.");

    public static string Amount(decimal amount, string currencyCode) =>
        $"{amount.ToString(AmountFormat, SpanishCulture)} {currencyCode}";

    public static string PickupAt(string businessName) => $"Podés retirarlo en {businessName}.";

    public static string InClass(string classGroupName) => $"Te lo entregan en la clase {classGroupName}.";

    public static string DeliveryForBranch(string? classGroupName) =>
        classGroupName is null ? "Entrega: retiro en la sede." : $"Entrega: en la clase {classGroupName}.";

    public const string ClassesCredited = "Las clases ya están cargadas en tu cuenta.";
    public const string WillNotifyWhenReady = "Te avisamos cuando los productos estén listos para entregar.";
}
