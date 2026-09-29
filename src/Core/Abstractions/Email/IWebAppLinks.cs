namespace ClassManager.Core.Abstractions.Email;

public interface IWebAppLinks
{
    string ResetPassword(string token);

    string AcceptInvitation(string token);

    string AcceptFamilyInvitation(string token);
}
