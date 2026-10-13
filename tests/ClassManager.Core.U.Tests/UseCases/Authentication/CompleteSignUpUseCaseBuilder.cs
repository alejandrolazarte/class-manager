using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.Domain.Organizations;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;
using ClassManager.Tenancy;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Authentication;

internal sealed class CompleteSignUpUseCaseBuilder
{
    public const string Token = "sign-up-token";
    public const string TokenHash = "sign-up-token-hash";

    public Guid UserId { get; } = Guid.CreateVersion7();
    public Organization? AddedOrganization { get; private set; }
    public OrganizationMember? AddedOrganizationMember { get; private set; }
    public Business? AddedBusiness { get; private set; }
    public BusinessMember? AddedMember { get; private set; }
    public Instructor? AddedInstructor { get; private set; }
    public Subscription? AddedSubscription { get; private set; }
    public LegalTermsAcceptance? AddedLegalTermsAcceptance { get; private set; }
    public Plan SignUpPlan { get; } = Plan.Create("free", displayOrder: 1, 0m, "USD", BillingPeriod.Monthly, isDefault: true);

    public PendingSignUp PendingSignUp { get; } = TestData.PendingSignUp(TokenHash);
    public Mock<IPendingSignUpRepository> PendingSignUps { get; } = new();
    public Mock<ISecretTokenGenerator> SecretTokens { get; } = new();
    public Mock<IIdentityService> Identity { get; } = new();
    public Mock<ITokenService> Tokens { get; } = new();
    public Mock<IOrganizationRepository> Organizations { get; } = new();
    public Mock<IOrganizationMemberRepository> OrganizationMembers { get; } = new();
    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IBusinessMemberRepository> BusinessMembers { get; } = new();
    public Mock<IInstructorRepository> Instructors { get; } = new();
    public Mock<ISubscriptionRepository> Subscriptions { get; } = new();
    public Mock<ILegalTermsAcceptanceRepository> LegalTermsAcceptances { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IUnitOfWorkTransaction> Transaction { get; } = new();
    public Mock<ITenantScope> TenantScope { get; } = new();

    public CompleteSignUpUseCaseBuilder()
    {
        SecretTokens.Setup(generator => generator.Hash(Token)).Returns(TokenHash);
        PendingSignUps
            .Setup(repository => repository.FindForUpdateByTokenHashAsync(TokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PendingSignUp);
        Identity
            .Setup(service => service.IsEmailRegisteredAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Identity
            .Setup(service => service.CreateOwnerAsync(It.IsAny<OwnerAccount>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(UserId));
        Tokens
            .Setup(service => service.IssueAsync(It.IsAny<SessionUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.IssuedTokens());
        Businesses
            .Setup(repository => repository.IsSlugTakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Organizations
            .Setup(repository => repository.Add(It.IsAny<Organization>()))
            .Callback<Organization>(organization => AddedOrganization = organization);
        OrganizationMembers
            .Setup(repository => repository.Add(It.IsAny<OrganizationMember>()))
            .Callback<OrganizationMember>(member => AddedOrganizationMember = member);
        Businesses
            .Setup(repository => repository.Add(It.IsAny<Business>()))
            .Callback<Business>(business => AddedBusiness = business);
        BusinessMembers
            .Setup(repository => repository.Add(It.IsAny<BusinessMember>()))
            .Callback<BusinessMember>(member => AddedMember = member);
        Instructors
            .Setup(repository => repository.Add(It.IsAny<Instructor>()))
            .Callback<Instructor>(instructor => AddedInstructor = instructor);
        Subscriptions
            .Setup(repository => repository.GetSignUpPlanAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(SignUpPlan);
        Subscriptions
            .Setup(repository => repository.Add(It.IsAny<Subscription>()))
            .Callback<Subscription>(subscription => AddedSubscription = subscription);
        LegalTermsAcceptances
            .Setup(repository => repository.Add(It.IsAny<LegalTermsAcceptance>()))
            .Callback<LegalTermsAcceptance>(acceptance => AddedLegalTermsAcceptance = acceptance);
        UnitOfWork
            .Setup(unitOfWork => unitOfWork.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Transaction.Object);
    }

    public static CompleteSignUpCommand ValidCommand() => new(Token, TestData.OwnerPassword);

    public CompleteSignUpUseCase Build() =>
        new(
            PendingSignUps.Object,
            Identity.Object,
            Tokens.Object,
            SecretTokens.Object,
            Organizations.Object,
            OrganizationMembers.Object,
            Businesses.Object,
            new InMemoryCountryRepository(),
            BusinessMembers.Object,
            Instructors.Object,
            Subscriptions.Object,
            LegalTermsAcceptances.Object,
            UnitOfWork.Object,
            TenantScope.Object,
            new FakeTimeProvider(TestData.Now));
}
