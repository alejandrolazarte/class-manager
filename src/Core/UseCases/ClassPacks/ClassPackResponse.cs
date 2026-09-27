using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record ClassPackResponse(Guid Id, string Name, int ClassCount, decimal Price, int? ValidityMonths, bool IsActive)
{
    public static ClassPackResponse From(ClassPack classPack) =>
        new(classPack.Id, classPack.Name, classPack.ClassCount, classPack.Price, classPack.ValidityMonths, classPack.IsActive);
}
