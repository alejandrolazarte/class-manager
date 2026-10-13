using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Authentication;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Authentication;

internal sealed class RequestSignUpUseCaseBuilder
{
    public const string Token = "sign-up-token";
    public const string TokenHash = "sign-up-token-hash";
    public const string CompleteSignUpLink = "https://app.example.com/complete-sign-up?token=sign-up-token";
    public const string SignInLink = "https://app.example.com/sign-in";

    public PendingSignUp? AddedPendingSignUp { get; private set; }
    public List<PendingSignUp> PreviousSignUps { get; } = [];

    public Mock<IIdentityService> Identity { get; } = new();
    public Mock<IPendingSignUpRepository> PendingSignUps { get; } = new();
    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<ISecretTokenGenerator> SecretTokens { get; } = new();
    public Mock<IEmailSender> EmailSender { get; } = new();
    public Mock<IWebAppLinks> WebAppLinks { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();

    public RequestSignUpUseCaseBuilder()
    {
        Identity
            .Setup(service => service.IsEmailRegisteredAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        PendingSignUps
            .Setup(repository => repository.Add(It.IsAny<PendingSignUp>()))
            .Callback<PendingSignUp>(pendingSignUp => AddedPendingSignUp = pendingSignUp);
        PendingSignUps
            .Setup(repository => repository.ListPendingForUpdateByEmailAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => PreviousSignUps);
        Businesses
            .Setup(repository => repository.IsSlugTakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        SecretTokens.Setup(generator => generator.Create()).Returns(new SecretToken(Token, TokenHash));
        WebAppLinks.Setup(links => links.CompleteSignUp(Token)).Returns(CompleteSignUpLink);
        WebAppLinks.Setup(links => links.SignIn()).Returns(SignInLink);
    }

    public static RequestSignUpCommand ValidCommand() =>
        new(
            TestData.OwnerFullName,
            TestData.OwnerEmail,
            TestData.BusinessName,
            TestData.ArgentinaCountryCode,
            TestData.BuenosAiresTimeZoneId,
            TestData.CurrencyCode,
            TestData.OwnerBirthDate,
            AcceptsLegalTerms: true);

    public RequestSignUpUseCase Build() =>
        new(
            Identity.Object,
            PendingSignUps.Object,
            Businesses.Object,
            new InMemoryCountryRepository(),
            SecretTokens.Object,
            EmailSender.Object,
            WebAppLinks.Object,
            UnitOfWork.Object,
            new FakeTimeProvider(TestData.Now));
}
