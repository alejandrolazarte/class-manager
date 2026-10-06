using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.UseCases.Instructors;

public sealed record UpdateInstructorRequest(string? FullName, string? Email = null)
{
    public UpdateInstructorCommand ToCommand(Guid instructorId) => new(instructorId, FullName, Email);
}

public sealed record UpdateInstructorCommand(Guid InstructorId, string? FullName, string? Email = null) : ICommand;

public sealed class UpdateInstructorUseCase(
    IInstructorRepository instructorRepository,
    IBusinessMemberRepository businessMemberRepository,
    IIdentityService identityService,
    IUnitOfWork unitOfWork)
    : IUseCase<UpdateInstructorCommand, InstructorResponse>
{
    private const string EmailUsedToSignInMessage = "The instructor signs in to the app with this email; only they can change it.";

    public async Task<Result<InstructorResponse>> ExecuteAsync(UpdateInstructorCommand command, CancellationToken cancellationToken)
    {
        var instructor = await instructorRepository.GetForUpdateAsync(command.InstructorId, cancellationToken);
        if (instructor is null)
        {
            return InstructorFailures.NotFound();
        }

        if (!instructor.HasEmail(command.Email)
            && await businessMemberRepository.FindUserIdByInstructorAsync(instructor.Id, cancellationToken) is { } userId)
        {
            var signInEmail = await SignInEmails.FirstAsync(identityService, [userId], cancellationToken);
            if (!string.Equals(command.Email?.Trim(), signInEmail, StringComparison.OrdinalIgnoreCase))
            {
                return new ResultError(InstructorErrorCodes.EmailUsedToSignIn, EmailUsedToSignInMessage, ErrorKind.Conflict)
                {
                    FieldName = nameof(UpdateInstructorCommand.Email),
                };
            }
        }

        var update = instructor.Update(command.FullName, command.Email);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        var existingInstructor = await instructorRepository.FindByNameAsync(instructor.FullName, cancellationToken);
        if (existingInstructor is not null && existingInstructor.Id != instructor.Id)
        {
            return InstructorFailures.NameTaken(existingInstructor.Id);
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return InstructorFailures.NameTaken(null);
        }

        return InstructorResponse.From(instructor);
    }
}
