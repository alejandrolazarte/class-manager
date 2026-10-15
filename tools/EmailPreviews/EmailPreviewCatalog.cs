using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Emails;
using ClassManager.Infrastructure.Email;
using ClassManager.Infrastructure.Notifications;
using ClassManager.Notifications.Email;

namespace ClassManager.EmailPreviews;

internal sealed record OrderItem(int Quantity, string Name, decimal Total);

internal sealed record EmailPreview(string Name, string Subject, EmailContent Content, bool UsesBusinessBrand);

internal static class EmailPreviewCatalog
{
    private const string WebAppUrl = "https://app.classmanager.example";
    private const string DemoToken = "demo-token";
    private const string NewRecipientEmail = "ana.perez.nueva@example.com";
    private const string ClientName = "Ana Pérez";
    private const string StudentName = "Tomás Pérez";
    private const string ClassGroupName = "Inicial martes y jueves";
    private const string CurrencyCode = "ARS";
    private const int GuardianConsentMinimumAge = 13;
    private const int OrderNumber = 1042;
    private const string LanguageSeparator = "-";

    public static Uri WebFontsUrl { get; } = new(WebAppUrl + WebAppLinks.WebFontsPath);

    public static IReadOnlyList<EmailPreview> All() => [.. EmailTexts.All.SelectMany(For)];

    private static IEnumerable<EmailPreview> For(IEmailTexts texts)
    {
        var productOrder = OrderWith(new OrderItem(2, "Remera talle M", 18000m), new OrderItem(1, "Gorra", 9500m));
        var classPackOrder = OrderWith(new OrderItem(1, "Pack 4 clases", 36000m));
        var studentAppOrders = WebAppUrl + WebAppLinks.StudentAppOrdersPath;
        var nameSuffix = texts.Language == AccountLanguage.Default ? string.Empty : LanguageSeparator + texts.Language;

        return
        [
            Business(
                "01-invitacion-al-equipo" + nameSuffix,
                texts.TeamInvitation(PreviewBrands.BusinessName, Link(WebAppLinks.AcceptInvitationPath))),
            Business(
                "02-invitacion-a-la-app" + nameSuffix,
                texts.StudentAppInvitation(PreviewBrands.BusinessName, Link(WebAppLinks.AcceptStudentAppInvitationPath))),
            Business(
                "03-autorizacion-del-responsable" + nameSuffix,
                texts.GuardianConsentRequest(
                    PreviewBrands.BusinessName, StudentName, GuardianConsentMinimumAge, Link(WebAppLinks.AuthorizeStudentAppPath))),
            Business(
                "04-nuevo-pedido-para-el-equipo" + nameSuffix,
                texts.OrderPlaced(productOrder, ClientName, texts.OrderDeliveryForBranch(ClassGroupName), WebAppUrl + WebAppLinks.TeamOrdersPath)),
            Business(
                "05-pago-confirmado-con-productos" + nameSuffix,
                texts.OrderPaid(productOrder, texts.OrderWillNotifyWhenReady, hasProducts: true, studentAppOrders)),
            Business(
                "06-pago-confirmado-listo-para-retirar" + nameSuffix,
                texts.OrderPaid(productOrder, texts.OrderPickupAt(PreviewBrands.BusinessName), hasProducts: true, studentAppOrders)),
            Business(
                "07-pago-confirmado-de-clases" + nameSuffix,
                texts.OrderPaid(classPackOrder, texts.OrderClassesCredited, hasProducts: false, studentAppOrders)),
            Business(
                "08-pedido-listo" + nameSuffix,
                texts.OrderReady(productOrder, texts.OrderInClass(ClassGroupName), studentAppOrders)),
            Business(
                "09-pedido-cancelado-sin-pago" + nameSuffix,
                texts.OrderCancelledUnpaid(productOrder, studentAppOrders)),
            Business(
                "10-pedido-cancelado-por-la-sede" + nameSuffix,
                texts.OrderCancelledByBranch(productOrder, studentAppOrders)),
            App(
                "11-elegir-contrasena" + nameSuffix,
                texts.PasswordReset(Link(WebAppLinks.ResetPasswordPath))),
            App(
                "12-confirmar-email-nuevo" + nameSuffix,
                texts.EmailChangeConfirmation(Link(WebAppLinks.ConfirmEmailChangePath))),
            App(
                "13-email-cambiado" + nameSuffix,
                texts.EmailChangedNotice(NewRecipientEmail)),
            App(
                "14-confirmar-registro" + nameSuffix,
                texts.SignUpConfirmation(PreviewBrands.BusinessName, Link(WebAppLinks.CompleteSignUpPath))),
            App(
                "15-registro-con-cuenta-existente" + nameSuffix,
                texts.SignUpAccountExists(WebAppUrl + WebAppLinks.SignInPath)),
        ];
    }

    private static OrderEmailDetails OrderWith(params OrderItem[] items) =>
        new(
            OrderNumber,
            PreviewBrands.BusinessName,
            [.. items.Select(item => OrderEmails.Line(item.Quantity, item.Name, item.Total, CurrencyCode))],
            OrderEmails.Amount(items.Sum(item => item.Total), CurrencyCode));

    private static string Link(string path) => WebAppUrl + path + DemoToken;

    private static EmailPreview Business(string name, EmailText text) => new(name, text.Subject, text.Content, UsesBusinessBrand: true);

    private static EmailPreview App(string name, EmailText text) => new(name, text.Subject, text.Content, UsesBusinessBrand: false);
}
