using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.UseCases.ClassPacks;

internal static class ClassPackResponses
{
    public static async Task<ClassPackResponse> ForAsync(
        ClassPack classPack,
        IClassPackRepository classPackRepository,
        IBusinessCalendarService businessCalendar,
        IDocumentStorageService documentStorage,
        CancellationToken cancellationToken) =>
        ClassPackResponse.From(
            classPack,
            documentStorage,
            await classPackRepository.ListPriceChangesAsync(cancellationToken),
            await businessCalendar.TodayAsync(cancellationToken));
}
