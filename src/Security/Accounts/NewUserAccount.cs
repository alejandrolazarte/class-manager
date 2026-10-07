namespace ClassManager.Security.Accounts;

public sealed record NewUserAccount(string Email, string Password, string FullName, DateOnly BirthDate);
