namespace ClassManager.Core.Abstractions.Email;

public interface IWebAppLinks
{
    string ResetPassword(string token);
}
