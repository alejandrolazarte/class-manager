using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

internal static class LegalTermsNotAccepted
{
    public static Result<TResponse> For<TResponse>(string fieldName) =>
        Result.Validation<TResponse>(
            AuthenticationErrorCodes.LegalTermsNotAcceptedMessage,
            AuthenticationErrorCodes.LegalTermsNotAccepted,
            fieldName);
}
