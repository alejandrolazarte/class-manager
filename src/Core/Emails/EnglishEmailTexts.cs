using System.Globalization;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Notifications.Email;

namespace ClassManager.Core.Emails;

internal sealed class EnglishEmailTexts : IEmailTexts
{
    private const string AccountEyebrow = "Your account";
    private const string AcceptInvitation = "Accept invitation";
    private const string ConfirmEmail = "Confirm email";
    private const string LinkValidForSevenDays = "The link works only once and expires in 7 days.";
    private const string LinkValidForOneDay = "The link works only once and expires in 24 hours.";
    private const string UnexpectedInvitationFooter = "If you weren't expecting this invitation, you can ignore this email.";
    private const string OrderNumberPrefix = "No. ";
    private const string StudentAppOrderEyebrowPrefix = "Order ";
    private const string TeamOrderEyebrowPrefix = "New order · ";
    private const string DeliveryLabel = "Hand-over";
    private const string StudentAppOrderAction = "See order in the app";
    private const string TeamOrderAction = "Open orders";
    private const string StudentAppOrderFooter = "You're getting this email because you placed an order in the app.";
    private const string OrderPaidSubject = "We received your payment";
    private const string OrderReadySubject = "Your order is ready";
    private const string OrderCancelledSubject = "Your order was cancelled";

    private static readonly EmailLayoutTexts EnglishLayout =
        new(AccountLanguage.English, "Total", "If the button doesn't work, copy this link into your browser:");

    private static readonly AppFeedbackEmailLabels AppFeedbackLabels = new("Business", "Email", "Role", "App");

    public string Language => AccountLanguage.English;

    public EmailLayoutTexts Layout => EnglishLayout;

    public string OrderClassesCredited => "The classes are already added to your account.";

    public string OrderWillNotifyWhenReady => "We'll let you know when the products are ready to hand over.";

    public string OrderClassesWhenHandedOver => "We'll add the classes to your account when we hand over your order.";

    public string OrderHandedOver => "We already handed over your order.";

    public EmailText TeamInvitation(string businessName, string acceptInvitationLink) =>
        new(
            $"{businessName} invites you to their team",
            Content(
                "Team invitation",
                "You were invited to join the team",
                $"You were invited to join the {businessName} team. Tap the button to accept.",
                UnexpectedInvitationFooter) with
            {
                Action = new EmailAction(AcceptInvitation, acceptInvitationLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForSevenDays),
            });

    public EmailText StudentAppInvitation(string businessName, string acceptInvitationLink) =>
        new(
            $"{businessName} invites you to their app",
            Content(
                "App invitation",
                "You were invited to the app",
                $"{businessName} invites you to their app to see classes, your balances and place orders. Tap the button to accept: if you already use the app with this email, you sign in with your usual account.",
                UnexpectedInvitationFooter) with
            {
                Action = new EmailAction(AcceptInvitation, acceptInvitationLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForSevenDays),
            });

    public EmailText GuardianConsentRequest(string businessName, string studentFullName, int minimumAge, string authorizeLink) =>
        new(
            $"{businessName} asks for your authorization for their app",
            Content(
                "App authorization",
                "Authorization request for the app",
                $"{businessName} wants to invite {studentFullName} to their app to see their classes. Since they're under {minimumAge}, we need your authorization.",
                "If you weren't expecting this request, you can ignore this email.") with
            {
                Action = new EmailAction("Review and authorize", authorizeLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForSevenDays),
            });

    public EmailText SignUpConfirmation(string businessName, string completeSignUpLink)
    {
        const string subject = "Confirm your email";
        return new(
            subject,
            Content(
                AccountEyebrow,
                subject,
                $"You asked to create the {businessName} account. Tap the button to confirm your email and choose your password.",
                "If you didn't ask for it, ignore this email: no account is created.") with
            {
                Action = new EmailAction(ConfirmEmail, completeSignUpLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForOneDay),
            });
    }

    public EmailText SignUpAccountExists(string signInLink)
    {
        const string subject = "You already have an account";
        return new(
            subject,
            Content(
                AccountEyebrow,
                subject,
                "Someone asked to create a new account with this email, but you already have one. Sign in with your email and your password.",
                "If you didn't ask for it, ignore this email: your account doesn't change.") with
            {
                Action = new EmailAction("Sign in", signInLink, ShowsLinkFallback: true),
                Note = new EmailNote("If you don't remember the password, tap \"Forgot your password?\" when signing in."),
            });
    }

    public EmailText PasswordReset(string resetPasswordLink)
    {
        const string subject = "Choose a new password";
        return new(
            subject,
            Content(
                AccountEyebrow,
                subject,
                "You asked to change your password. Tap the button to choose a new one.",
                "If you didn't ask for it, ignore this email: your password doesn't change.") with
            {
                Action = new EmailAction("Choose password", resetPasswordLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForOneDay),
            });
    }

    public EmailText EmailChangeConfirmation(string confirmEmailChangeLink)
    {
        const string subject = "Confirm your new email";
        return new(
            subject,
            Content(
                AccountEyebrow,
                subject,
                "You asked to use this email to sign in to the app. Tap the button to confirm it.",
                "If you didn't ask for it, ignore this email: your account stays the same.") with
            {
                Action = new EmailAction(ConfirmEmail, confirmEmailChangeLink, ShowsLinkFallback: true),
                Note = new EmailNote(LinkValidForOneDay),
            });
    }

    public EmailText EmailChangedNotice(string newEmail)
    {
        const string subject = "You changed your account's email";
        return new(
            subject,
            Content(
                AccountEyebrow,
                subject,
                $"From now on you sign in to the app with {newEmail}.",
                "If it wasn't you, let the place where you take classes or work know right away."));
    }

    public EmailText PasskeyAddedNotice(string passkeyName)
    {
        const string subject = "A passkey was added to your account";
        return new(
            subject,
            Content(
                AccountEyebrow,
                subject,
                $"From now on you can sign in to the app with the passkey {passkeyName}, using that device's fingerprint, face or PIN.",
                "If it wasn't you, remove it from My profile and change your password."));
    }

    public EmailText OrderPlaced(OrderEmailDetails order, string clientName, string? delivery, string teamOrdersLink) =>
        new(
            $"New order {OrderNumber(order.Number)} from {clientName}",
            Content(
                TeamOrderEyebrowPrefix + OrderNumber(order.Number),
                $"{clientName} placed an order from the app",
                "It's paid at the venue. When you collect it, confirm the payment in Orders.",
                $"You're getting this email because you manage the orders of {order.BusinessName}.") with
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
            $"{order.BusinessName} confirmed the payment of your order.",
            new EmailNote(nextStep, hasProducts ? DeliveryLabel : null),
            studentAppOrdersLink);

    public EmailText OrderReady(OrderEmailDetails order, string whereToGetIt, string studentAppOrdersLink) =>
        StudentAppOrder(
            order,
            OrderReadySubject,
            $"Your order from {order.BusinessName} is ready to hand over.",
            new EmailNote(whereToGetIt, DeliveryLabel),
            studentAppOrdersLink);

    public EmailText OrderCancelledUnpaid(OrderEmailDetails order, string studentAppOrdersLink) =>
        StudentAppOrder(
            order,
            OrderCancelledSubject,
            $"Your order from {order.BusinessName} was cancelled because it wasn't paid within 7 days.",
            new EmailNote("If you still want it, you can order it again from the app."),
            studentAppOrdersLink);

    public EmailText OrderCancelledByBranch(OrderEmailDetails order, string studentAppOrdersLink) =>
        StudentAppOrder(
            order,
            OrderCancelledSubject,
            $"{order.BusinessName} cancelled your order.",
            new EmailNote("If you have questions, ask at the venue."),
            studentAppOrdersLink);

    public EmailText AppFeedback(AppFeedbackEmailDetails feedback) =>
        new(
            $"Feedback from {feedback.SenderFullName} ({feedback.BusinessName})",
            Content(
                "App feedback",
                $"Feedback from {feedback.SenderFullName}",
                feedback.Message,
                "Reply to this email to answer them.") with
            {
                Lines = feedback.Lines(AppFeedbackLabels),
            });

    public string OrderNumber(int orderNumber) => OrderNumberPrefix + orderNumber.ToString(CultureInfo.InvariantCulture);

    public string OrderPickupAt(string businessName) => $"You can pick it up at {businessName}.";

    public string OrderInClass(string classGroupName) => $"You get it in the {classGroupName} class.";

    public string OrderDeliveryForBranch(string? classGroupName) =>
        classGroupName is null ? "Pick up at the venue." : $"In the {classGroupName} class.";

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
