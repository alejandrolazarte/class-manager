using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_UpdateClient_with_invalid_phone_number;

public sealed class Then_returns_phone_number_field_error
{
    [Fact]
    public async Task Then_returns_phone_number_field_error_Run()
    {
        var builder = new UpdateClientUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand() with { PhoneNumber = "call me" }, CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(UpdateClientCommand.PhoneNumber));
        builder.Client.FullName.ShouldBe(TestData.ClientFullName);
    }
}
