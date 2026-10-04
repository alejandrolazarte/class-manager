using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record ListStudentAppOrdersQuery : IQuery;

public sealed class ListStudentAppOrdersUseCase(IStudentAppAccess studentAppAccess, IOrderRepository orderRepository, IClassGroupRepository classGroupRepository)
    : IUseCase<ListStudentAppOrdersQuery, IReadOnlyList<StudentAppOrderResponse>>
{
    private const int OrderLimit = 50;

    public async Task<Result<IReadOnlyList<StudentAppOrderResponse>>> ExecuteAsync(ListStudentAppOrdersQuery command, CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return StudentAppFailures.NoAccess();
        }

        var orders = await orderRepository.ListAsync(new OrderSearchCriteria(access.ClientId, false, OrderLimit), null, cancellationToken);
        return Result.Success(await StudentAppOrderResponses.OfAsync(orders, classGroupRepository, cancellationToken));
    }
}
