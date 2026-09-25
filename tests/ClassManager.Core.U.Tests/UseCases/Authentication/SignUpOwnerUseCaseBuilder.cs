using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Tenancy;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Authentication;

internal sealed class SignUpOwnerUseCaseBuilder
{
    public Guid UserId { get; } = Guid.CreateVersion7();
    public Business? AddedBusiness { get; private set; }
    public BusinessMember? AddedMember { get; private set; }
    public Instructor? AddedInstructor { get; private set; }

    public Mock<IIdentityService> Identity { get; } = new();
    public Mock<ITokenService> Tokens { get; } = new();
    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IBusinessMemberRepository> BusinessMembers { get; } = new();
    public Mock<IInstructorRepository> Instructors { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IUnitOfWorkTransaction> Transaction { get; } = new();
    public Mock<ITenantScope> TenantScope { get; } = new();

    public SignUpOwnerUseCaseBuilder()
    {
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
        Businesses
            .Setup(repository => repository.Add(It.IsAny<Business>()))
            .Callback<Business>(business => AddedBusiness = business);
        BusinessMembers
            .Setup(repository => repository.Add(It.IsAny<BusinessMember>()))
            .Callback<BusinessMember>(member => AddedMember = member);
        Instructors
            .Setup(repository => repository.Add(It.IsAny<Instructor>()))
            .Callback<Instructor>(instructor => AddedInstructor = instructor);
        UnitOfWork
            .Setup(unitOfWork => unitOfWork.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Transaction.Object);
    }

    public static SignUpOwnerCommand ValidCommand() =>
        new(
            TestData.OwnerFullName,
            TestData.OwnerEmail,
            TestData.OwnerPassword,
            TestData.BusinessName,
            TestData.BuenosAiresTimeZoneId,
            TestData.CurrencyCode,
            TestData.DefaultCountryCallingCode);

    public SignUpOwnerUseCase Build() =>
        new(
            Identity.Object,
            Tokens.Object,
            Businesses.Object,
            BusinessMembers.Object,
            Instructors.Object,
            UnitOfWork.Object,
            TenantScope.Object,
            new FakeTimeProvider(TestData.Now));
}
