using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Clients;

public sealed record UpdateClientRequest(string? FullName, string? PhoneNumber, string? Email, string? Notes)
{
    public UpdateClientCommand ToCommand(Guid clientId) => new(clientId, FullName, PhoneNumber, Email, Notes);
}

public sealed record UpdateClientCommand(
    Guid ClientId,
    string? FullName,
    string? PhoneNumber,
    string? Email,
    string? Notes) : ICommand;

public sealed class UpdateClientUseCase(
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    IClientAccountRepository clientAccountRepository,
    IIdentityService identityService)
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

        var existingClient = await clientRepository.FindByPhoneNumberAsync(phoneNumber.Value!, cancellationToken);
        if (existingClient is not null && existingClient.Id != client.Id)
        {
            return PhoneNumberTaken(existingClient.Id);
        }

        if (!client.HasEmail(command.Email))
        {
            var accountUserIds = await clientAccountRepository.ListUserIdsByClientAsync(client.Id, cancellationToken);
            var signInEmail = await SignInEmails.FirstAsync(identityService, accountUserIds, cancellationToken);
            if (accountUserIds.Count > 0 && !string.Equals(command.Email?.Trim(), signInEmail, StringComparison.OrdinalIgnoreCase))
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

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            var winnerClient = await clientRepository.FindByPhoneNumberAsync(phoneNumber.Value!, cancellationToken);
            return PhoneNumberTaken(winnerClient?.Id);
        }

        return ClientResponse.From(client);
    }

    private static Result<ClientResponse> PhoneNumberTaken(Guid? existingClientId) =>
        Result.Conflict<ClientResponse>(
            PhoneNumberTakenMessage,
            ClientErrorCodes.PhoneNumberTaken,
            new Dictionary<string, object?> { [ClientErrorCodes.ExistingClientIdDetail] = existingClientId });
}
