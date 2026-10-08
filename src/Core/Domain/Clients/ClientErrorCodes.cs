namespace ClassManager.Core.Domain.Clients;

public static class ClientErrorCodes
{
    public const string PhoneNumberTaken = "client.phone_number_taken";
    public const string NotFound = "client.not_found";
    public const string EmailUsedToSignIn = "client.email_used_to_sign_in";
    public const string ContactMustBeAdult = "client.contact_must_be_adult";
    public const string ExistingClientIdDetail = "clientId";
}
