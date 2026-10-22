using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.Clients;

public sealed record UpdateClientRequest(string? FullName, string? PhoneNumber, string? Email, string? Notes)
{
    public UpdateClientCommand ToCommand(long clientId) => new(clientId, FullName, PhoneNumber, Email, Notes);
}

public sealed record UpdateClientCommand(
    long ClientId,
    [property: AuditPersonal] string? FullName,
    [property: AuditPersonal] string? PhoneNumber,
    [property: AuditPersonal] string? Email,
    [property: AuditPersonal] string? Notes) : ICommand;

public sealed class UpdateClientUseCase(
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    IClientAccountRepository clientAccountRepository,
    IIdentityService identityService,
    IUserRepository userRepository,
    IPeopleLock peopleLock)
    : IUseCase<UpdateClientCommand, ClientResponse>
{
    private const string NotFoundMessage = "The client does not exist.";
    private const string PhoneNumberTakenMessage = "Another client is already registered with this phone number.";
    private const string EmailUsedToSignInMessage = "The client signs in to the app with this email; only they can change it.";

    public async Task<Result<ClientResponse>> ExecuteAsync(UpdateClientCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<ClientResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var client = await clientRepository.GetForUpdateAsync(command.ClientId, cancellationToken);
        if (client is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [client.Id], cancellationToken))
        {
            return Result.NotFound<ClientResponse>(NotFoundMessage, ClientErrorCodes.NotFound);
        }

        var phoneNumber = PhoneNumber.Create(command.PhoneNumber, business.DefaultCountryCallingCode);
        if (phoneNumber.IsFailure)
        {
            return phoneNumber.Error! with { FieldName = nameof(UpdateClientCommand.PhoneNumber) };
        }

        await peopleLock.LockAsync(cancellationToken);
        var existingClient = await clientRepository.FindByPhoneNumberAsync(phoneNumber.Value!, cancellationToken);
        if (existingClient is not null && existingClient.Id != client.Id)
        {
            return PhoneNumberTaken(existingClient.Id);
        }

        if (!client.User.HasEmail(command.Email))
        {
            var accounts = await clientAccountRepository.ListByClientAsync(client.Id, cancellationToken);
            var accountIds = accounts
                .Where(account => account.StudentId is null)
                .Select(account => account.User.AccountId)
                .OfType<Guid>()
                .ToList();
            var signInEmail = await SignInEmails.FirstAsync(identityService, accountIds, cancellationToken);
            if (accountIds.Count > 0 && !string.Equals(command.Email?.Trim(), signInEmail, StringComparison.OrdinalIgnoreCase))
            {
                return new ResultError(ClientErrorCodes.EmailUsedToSignIn, EmailUsedToSignInMessage, ErrorKind.Conflict)
                {
                    FieldName = nameof(UpdateClientCommand.Email),
                };
            }
        }

        var update = client.Update(command.FullName, phoneNumber.Value!, command.Email, command.Notes);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        if (await PeopleEmails.TakenErrorAsync(userRepository, [(client.User, nameof(UpdateClientCommand.Email))], cancellationToken) is { } emailTaken)
        {
            return emailTaken;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ClientResponse.From(client);
    }

    private static Result<ClientResponse> PhoneNumberTaken(long? existingClientId) =>
        Result.Conflict<ClientResponse>(
            PhoneNumberTakenMessage,
            ClientErrorCodes.PhoneNumberTaken,
            new Dictionary<string, object?> { [ClientErrorCodes.ExistingClientIdDetail] = existingClientId });
}
