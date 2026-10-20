using System.Globalization;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Notifications.Email;

namespace ClassManager.Core.Emails;

internal sealed class SpanishEmailTexts : IEmailTexts
{
    private const string AccountEyebrow = "Tu cuenta";
    private const string AcceptInvitation = "Aceptar invitación";
    private const string ConfirmEmail = "Confirmar email";
    private const string LinkValidForSevenDays = "El link sirve una sola vez y vence en 7 días.";
    private const string LinkValidForOneDay = "El link sirve una sola vez y vence en 24 horas.";
    private const string UnexpectedInvitationFooter = "Si no esperabas esta invitación, podés ignorar este mail.";
    private const string OrderNumberPrefix = "n.º ";
    private const string StudentAppOrderEyebrowPrefix = "Pedido ";
    private const string TeamOrderEyebrowPrefix = "Nuevo pedido · ";
    private const string DeliveryLabel = "Entrega";
    private const string StudentAppOrderAction = "Ver pedido en la app";
    private const string TeamOrderAction = "Abrir pedidos";
    private const string StudentAppOrderFooter = "Recibís este mail porque hiciste un pedido en la app.";
    private const string OrderPaidSubject = "Recibimos tu pago";
    private const string OrderReadySubject = "Tu pedido está listo";
    private const string OrderCancelledSubject = "Se canceló tu pedido";

    private static readonly AppFeedbackEmailLabels AppFeedbackLabels = new("Negocio", "Email", "Rol", "App");

    public string Language => AccountLanguage.Spanish;

    public EmailLayoutTexts Layout => EmailLayoutTexts.Spanish;

    public string OrderClassesCredited => "Las clases ya están cargadas en tu cuenta.";

    public string OrderWillNotifyWhenReady => "Te avisamos cuando los productos estén listos para entregar.";

    public EmailText TeamInvitation(string businessName, string acceptInvitationLink) =>
        new(
            $"{businessName} te invita a su equipo",
            Content(
                "Invitación al equipo",
                "Te invitaron a sumarte al equipo",
                $"Te invitaron a sumarte al equipo de {businessName}. Tocá el botón para aceptar.",
                UnexpectedInvitationFooter) with
            {
                Action = new EmailAction(AcceptInvitation, acceptInvitationLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForSevenDays),
            });

    public EmailText StudentAppInvitation(string businessName, string acceptInvitationLink) =>
        new(
            $"{businessName} te invita a su app",
            Content(
                "Invitación a la app",
                "Te invitaron a la app",
                $"{businessName} te invita a su app para ver las clases, tus saldos y hacer pedidos. Tocá el botón para aceptar: si ya usás la app con este email, entrás con tu cuenta de siempre.",
                UnexpectedInvitationFooter) with
            {
                Action = new EmailAction(AcceptInvitation, acceptInvitationLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForSevenDays),
            });

    public EmailText GuardianConsentRequest(string businessName, string studentFullName, int minimumAge, string authorizeLink) =>
        new(
            $"{businessName} pide tu autorización para su app",
            Content(
                "Autorización para la app",
                "Pedido de autorización para la app",
                $"{businessName} quiere invitar a {studentFullName} a su app para ver sus clases. Como es menor de {minimumAge} años, necesitamos tu autorización.",
                "Si no esperabas este pedido, podés ignorar este mail.") with
            {
                Action = new EmailAction("Revisar y autorizar", authorizeLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForSevenDays),
            });

    public EmailText SignUpConfirmation(string businessName, string completeSignUpLink)
    {
        const string subject = "Confirmá tu email";
        return new(
            subject,
            Content(
                AccountEyebrow,
                subject,
                $"Pediste crear la cuenta de {businessName}. Tocá el botón para confirmar tu email y elegir tu contraseña.",
                "Si no lo pediste, ignorá este mail: no se crea ninguna cuenta.") with
            {
                Action = new EmailAction(ConfirmEmail, completeSignUpLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForOneDay),
            });
    }

    public EmailText SignUpAccountExists(string signInLink)
    {
        const string subject = "Ya tenés una cuenta";
        return new(
            subject,
            Content(
                AccountEyebrow,
                subject,
                "Alguien pidió crear una cuenta nueva con este email, pero ya tenés una. Entrá con tu email y tu contraseña.",
                "Si no lo pediste, ignorá este mail: tu cuenta no cambia.") with
            {
                Action = new EmailAction("Ingresar", signInLink, ShowsLinkFallback: true),
                Note = new EmailNote("Si no recordás la contraseña, tocá \"¿Olvidaste tu contraseña?\" al ingresar."),
            });
    }

    public EmailText PasswordReset(string resetPasswordLink)
    {
        const string subject = "Elegí una contraseña nueva";
        return new(
            subject,
            Content(
                AccountEyebrow,
                subject,
                "Pediste cambiar tu contraseña. Tocá el botón para elegir una nueva.",
                "Si no lo pediste, ignorá este mail: tu contraseña no cambia.") with
            {
                Action = new EmailAction("Elegir contraseña", resetPasswordLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForOneDay),
            });
    }

    public EmailText EmailChangeConfirmation(string confirmEmailChangeLink)
    {
        const string subject = "Confirmá tu nuevo email";
        return new(
            subject,
            Content(
                AccountEyebrow,
                subject,
                "Pediste usar este email para entrar a la app. Tocá el botón para confirmarlo.",
                "Si no lo pediste, ignorá este mail: tu cuenta sigue igual.") with
            {
                Action = new EmailAction(ConfirmEmail, confirmEmailChangeLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForOneDay),
            });
    }

    public EmailText EmailChangedNotice(string newEmail)
    {
        const string subject = "Cambiaste el email de tu cuenta";
        return new(
            subject,
            Content(
                AccountEyebrow,
                subject,
                $"Desde ahora entrás a la app con {newEmail}.",
                "Si no fuiste vos, avisá enseguida al lugar donde tomás clases o trabajás."));
    }

    public EmailText OrderPlaced(OrderEmailDetails order, string clientName, string? delivery, string teamOrdersLink) =>
        new(
            $"Nuevo pedido {OrderNumber(order.Number)} de {clientName}",
            Content(
                TeamOrderEyebrowPrefix + OrderNumber(order.Number),
                $"{clientName} hizo un pedido desde la app",
                "Se paga en la sede. Cuando lo cobres, confirmá el pago en Pedidos.",
                $"Recibís este mail porque gestionás los pedidos de {order.BusinessName}.") with
            {
                Lines = order.Lines,
                Total = order.Total,
                Note = delivery is null ? null : new EmailNote(delivery, DeliveryLabel),
                Action = new EmailAction(TeamOrderAction, teamOrdersLink),
            });

    public EmailText OrderPaid(OrderEmailDetails order, string nextStep, bool hasProducts, string studentAppOrdersLink) =>
        StudentAppOrder(
            order,
            OrderPaidSubject,
            $"{order.BusinessName} confirmó el pago de tu pedido.",
            new EmailNote(nextStep, hasProducts ? DeliveryLabel : null),
            studentAppOrdersLink);

    public EmailText OrderReady(OrderEmailDetails order, string whereToGetIt, string studentAppOrdersLink) =>
        StudentAppOrder(
            order,
            OrderReadySubject,
            $"Tu pedido de {order.BusinessName} ya está listo para entregar.",
            new EmailNote(whereToGetIt, DeliveryLabel),
            studentAppOrdersLink);

    public EmailText OrderCancelledUnpaid(OrderEmailDetails order, string studentAppOrdersLink) =>
        StudentAppOrder(
            order,
            OrderCancelledSubject,
            $"Tu pedido de {order.BusinessName} se canceló porque no se pagó en 7 días.",
            new EmailNote("Si todavía lo querés, podés pedirlo de nuevo desde la app."),
            studentAppOrdersLink);

    public EmailText OrderCancelledByBranch(OrderEmailDetails order, string studentAppOrdersLink) =>
        StudentAppOrder(
            order,
            OrderCancelledSubject,
            $"{order.BusinessName} canceló tu pedido.",
            new EmailNote("Si tenés dudas, consultá en la sede."),
            studentAppOrdersLink);

    public EmailText AppFeedback(AppFeedbackEmailDetails feedback) =>
        new(
            $"Comentario de {feedback.SenderFullName} ({feedback.BusinessName})",
            Content(
                "Comentarios de la app",
                $"Comentario de {feedback.SenderFullName}",
                feedback.Message,
                "Respondé este mail para contestarle.") with
            {
                Lines = feedback.Lines(AppFeedbackLabels),
            });

    public string OrderNumber(int orderNumber) => OrderNumberPrefix + orderNumber.ToString(CultureInfo.InvariantCulture);

    public string OrderPickupAt(string businessName) => $"Podés retirarlo en {businessName}.";

    public string OrderInClass(string classGroupName) => $"Te lo entregan en la clase {classGroupName}.";

    public string OrderDeliveryForBranch(string? classGroupName) =>
        classGroupName is null ? "Retiro en la sede." : $"En la clase {classGroupName}.";

    private EmailText StudentAppOrder(OrderEmailDetails order, string subject, string intro, EmailNote note, string studentAppOrdersLink) =>
        new(
            subject,
            Content(StudentAppOrderEyebrowPrefix + OrderNumber(order.Number), subject, intro, StudentAppOrderFooter) with
            {
                Lines = order.Lines,
                Total = order.Total,
                Note = note,
                Action = new EmailAction(StudentAppOrderAction, studentAppOrdersLink),
            });

    private EmailContent Content(string eyebrow, string title, string intro, string footerNote) =>
        new(eyebrow, title, intro, footerNote) { Layout = Layout };
}
