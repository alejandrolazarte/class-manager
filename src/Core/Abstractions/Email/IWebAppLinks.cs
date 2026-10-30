namespace ClassManager.Core.Abstractions.Email;

public interface IWebAppLinks
{
    string SignIn();

    string CompleteSignUp(string token, string email);

    string ResetPassword(string token, string email);

    string ConfirmEmailChange(string token);

    string AcceptInvitation(string token);

    string AcceptStudentAppInvitation(string token);

    string AuthorizeStudentApp(string token);

    string StudentAppOrders();

    string TeamOrders();

    string PlanSettings();

    string PlanCheckoutSucceeded();

    string PlanCheckoutCancelled();
}
