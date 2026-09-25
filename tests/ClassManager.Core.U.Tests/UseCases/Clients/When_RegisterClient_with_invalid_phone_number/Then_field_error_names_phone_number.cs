using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_invalid_phone_number;

public sealed class Then_field_error_names_phone_number
{
    [Fact]
    public async Task Then_field_error_names_phone_number_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var command = RegisterClientUseCaseBuilder.ValidCommand() with { PhoneNumber = "call me maybe" };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(RegisterClientCommand.PhoneNumber));
    }
}
