using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Clients;

public sealed record RegisterClientCommand(
    string? FullName,
    string? PhoneNumber,
    string? Email,
    string? Notes);

public sealed class RegisterClientUseCase(
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<RegisterClientCommand, ClientResponse>
{
    private const string PhoneNumberTakenMessage = "A client with this phone number is already registered.";

    public async Task<Result<ClientResponse>> ExecuteAsync(RegisterClientCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<ClientResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var phoneNumber = PhoneNumber.Create(command.PhoneNumber, business.DefaultCountryCallingCode);
        if (phoneNumber.IsFailure)
        {
            return phoneNumber.Error! with { FieldName = nameof(RegisterClientCommand.PhoneNumber) };
        }

        var client = Client.Create(command.FullName, phoneNumber.Value!, command.Email, command.Notes, timeProvider.GetUtcNow());
        if (client.IsFailure)
        {
            return client.Error!;
        }

        var existingClient = await clientRepository.FindByPhoneNumberAsync(phoneNumber.Value!, cancellationToken);
        if (existingClient is not null)
        {
            return PhoneNumberTaken(existingClient.Id);
        }

        clientRepository.Add(client.Value!);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            var winnerClient = await clientRepository.FindByPhoneNumberAsync(phoneNumber.Value!, cancellationToken);
            return PhoneNumberTaken(winnerClient?.Id);
        }

        return ClientResponse.From(client.Value!);
    }

    private static Result<ClientResponse> PhoneNumberTaken(Guid? existingClientId) =>
        Result.Conflict<ClientResponse>(
            PhoneNumberTakenMessage,
            ClientErrorCodes.PhoneNumberTaken,
            new Dictionary<string, object?> { [ClientErrorCodes.ExistingClientIdDetail] = existingClientId });
}
