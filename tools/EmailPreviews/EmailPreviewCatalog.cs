using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.UseCases.Accounts;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Members;
using ClassManager.Core.UseCases.StudentApp;
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
    private const string RecipientEmail = "ana.perez@example.com";
    private const string NewRecipientEmail = "ana.perez.nueva@example.com";
    private const string ClientName = "Ana Pérez";
    private const string StudentName = "Tomás Pérez";
    private const string ClassGroupName = "Inicial martes y jueves";
    private const string CurrencyCode = "ARS";
    private const int GuardianConsentMinimumAge = 13;
    private const int OrderNumber = 1042;

    public static Uri WebFontsUrl { get; } = new(WebAppUrl + WebAppLinks.WebFontsPath);

    private static readonly Guid BusinessId = Guid.Parse("0192f0c0-0000-7000-8000-0000000000aa");

    public static IReadOnlyList<EmailPreview> All()
    {
        var productOrder = OrderWith(new OrderItem(2, "Remera talle M", 18000m), new OrderItem(1, "Gorra", 9500m));
        var classPackOrder = OrderWith(new OrderItem(1, "Pack 4 clases", 36000m));
        var studentAppOrders = WebAppUrl + WebAppLinks.StudentAppOrdersPath;

        return
        [
            Business(
                "01-invitacion-al-equipo",
                InviteMemberUseCase.EmailSubjectFor(PreviewBrands.BusinessName),
                InviteMemberUseCase.EmailContentFor(PreviewBrands.BusinessName, Link(WebAppLinks.AcceptInvitationPath))),
            Business(
                "02-invitacion-a-la-app",
                InviteStudentAppUseCase.EmailSubjectFor(PreviewBrands.BusinessName),
                InviteStudentAppUseCase.EmailContentFor(PreviewBrands.BusinessName, Link(WebAppLinks.AcceptStudentAppInvitationPath))),
            Business(
                "03-autorizacion-del-responsable",
                InviteStudentAppUseCase.GuardianEmailSubjectFor(PreviewBrands.BusinessName),
                InviteStudentAppUseCase.GuardianEmailContentFor(
                    PreviewBrands.BusinessName, StudentName, GuardianConsentMinimumAge, Link(WebAppLinks.AuthorizeStudentAppPath))),
            Business(
                "04-nuevo-pedido-para-el-equipo",
                OrderEmails.Placed(productOrder, ClientName, OrderEmails.DeliveryForBranch(ClassGroupName), WebAppUrl + WebAppLinks.TeamOrdersPath)),
            Business(
                "05-pago-confirmado-con-productos",
                OrderEmails.Paid(productOrder, OrderEmails.WillNotifyWhenReady, hasProducts: true, studentAppOrders)),
            Business(
                "06-pago-confirmado-listo-para-retirar",
                OrderEmails.Paid(productOrder, OrderEmails.PickupAt(PreviewBrands.BusinessName), hasProducts: true, studentAppOrders)),
            Business(
                "07-pago-confirmado-de-clases",
                OrderEmails.Paid(classPackOrder, OrderEmails.ClassesCredited, hasProducts: false, studentAppOrders)),
            Business(
                "08-pedido-listo",
                OrderEmails.Ready(productOrder, OrderEmails.InClass(ClassGroupName), studentAppOrders)),
            Business(
                "09-pedido-cancelado-sin-pago",
                OrderEmails.CancelledUnpaid(productOrder, studentAppOrders)),
            Business(
                "10-pedido-cancelado-por-la-sede",
                OrderEmails.CancelledByBranch(productOrder, studentAppOrders)),
            App(
                "11-elegir-contrasena",
                RequestPasswordResetUseCase.EmailSubject,
                RequestPasswordResetUseCase.EmailContentFor(Link(WebAppLinks.ResetPasswordPath))),
            App(
                "12-confirmar-email-nuevo",
                RequestEmailChangeUseCase.EmailSubject,
                RequestEmailChangeUseCase.EmailContentFor(Link(WebAppLinks.ConfirmEmailChangePath))),
            App(
                "13-email-cambiado",
                ConfirmEmailChangeUseCase.NoticeSubject,
                ConfirmEmailChangeUseCase.NoticeContentFor(NewRecipientEmail)),
        ];
    }

    private static OrderEmailContext OrderWith(params OrderItem[] items) =>
        new(
            RecipientEmail,
            OrderNumber,
            BusinessId,
            PreviewBrands.BusinessName,
            [.. items.Select(item => OrderEmails.Line(item.Quantity, item.Name, item.Total, CurrencyCode))],
            OrderEmails.Amount(items.Sum(item => item.Total), CurrencyCode));

    private static string Link(string path) => WebAppUrl + path + DemoToken;

    private static EmailPreview Business(string name, string subject, EmailContent content) => new(name, subject, content, UsesBusinessBrand: true);

    private static EmailPreview Business(string name, EmailMessage message) => new(name, message.Subject, message.Content, UsesBusinessBrand: true);

    private static EmailPreview App(string name, string subject, EmailContent content) => new(name, subject, content, UsesBusinessBrand: false);
}
