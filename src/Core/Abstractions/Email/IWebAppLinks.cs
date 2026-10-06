namespace ClassManager.Core.Abstractions.Email;

public interface IWebAppLinks
{
    string ResetPassword(string token);

    string ConfirmEmailChange(string token);

    string AcceptInvitation(string token);

    string AcceptStudentAppInvitation(string token);

    string StudentAppOrders();

    string TeamOrders();
}
