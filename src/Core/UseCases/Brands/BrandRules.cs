using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Brands;

internal static class BrandRules
{
    public static Result<T> BusinessNotFound<T>() =>
        Result.Unauthorized<T>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
}
