using ClassManager.Notifications.Email;

namespace ClassManager.Core.Emails;

public interface IEmailTexts
{
    string Language { get; }

    EmailLayoutTexts Layout { get; }

    string OrderClassesCredited { get; }

    string OrderWillNotifyWhenReady { get; }

    string OrderClassesWhenHandedOver { get; }

    string OrderHandedOver { get; }

    EmailText TeamInvitation(string businessName, string acceptInvitationLink);

    EmailText StudentAppInvitation(string businessName, string acceptInvitationLink);

    EmailText GuardianConsentRequest(string businessName, string studentFullName, int minimumAge, string authorizeLink);

    EmailText SignUpConfirmation(string businessName, string completeSignUpLink);

    EmailText SignUpAccountExists(string signInLink);

    EmailText PasswordReset(string resetPasswordLink);

    EmailText EmailChangeConfirmation(string confirmEmailChangeLink);

    EmailText EmailChangedNotice(string newEmail);

    EmailText OrderPlaced(OrderEmailDetails order, string clientName, string? delivery, string teamOrdersLink);

    EmailText OrderPaid(OrderEmailDetails order, string nextStep, bool hasProducts, string studentAppOrdersLink);

    EmailText OrderReady(OrderEmailDetails order, string whereToGetIt, string studentAppOrdersLink);

    EmailText OrderCancelledUnpaid(OrderEmailDetails order, string studentAppOrdersLink);

    EmailText OrderCancelledByBranch(OrderEmailDetails order, string studentAppOrdersLink);

    EmailText AppFeedback(AppFeedbackEmailDetails feedback);

    string OrderNumber(int orderNumber);

    string OrderPickupAt(string businessName);

    string OrderInClass(string classGroupName);

    string OrderDeliveryForBranch(string? classGroupName);
}
