using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.Endpoints;

internal static class ClientEndpoints
{
    public static IEndpointRouteBuilder MapClientEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var clients = endpoints.MapGroup(ApiRoutes.Clients);

        clients.MapPost("/", RegisterClientAsync);
        clients.MapGet(ApiRoutes.ClientById, GetClientAsync);
        clients.MapGet("/", SearchClientsAsync);

        return endpoints;
    }

    private static async Task<IResult> RegisterClientAsync(
        RegisterClientCommand command,
        IUseCase<RegisterClientCommand, ClientResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(client => TypedResults.Created($"{ApiRoutes.Clients}/{client.Id}", client));
    }

    private static async Task<IResult> GetClientAsync(
        Guid clientId,
        IUseCase<GetClientQuery, ClientResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetClientQuery(clientId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SearchClientsAsync(
        string? search,
        int? limit,
        IUseCase<SearchClientsQuery, IReadOnlyList<ClientResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new SearchClientsQuery(search, limit), cancellationToken);

        return result.ToOkResult();
    }
}
