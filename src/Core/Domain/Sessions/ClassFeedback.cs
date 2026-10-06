using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Sessions;

public sealed class ClassFeedback : ITenantOwned, ISoftDeletable
{
    public const int TextMaxLength = 500;

    private const string TextRequiredMessage = "Write the comment.";
    private const string TextLengthMessage = "The comment must be at most 500 characters.";

    private ClassFeedback()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClassSessionId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid InstructorId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Result<ClassFeedback> Create(
        Guid classSessionId, Guid studentId, Guid instructorId, string? text, DateTimeOffset createdAt)
    {
        var validation = Validate(text);
        if (validation.IsFailure)
        {
            return validation.Error!;
        }

        return new ClassFeedback
        {
            Id = Guid.CreateVersion7(),
            ClassSessionId = classSessionId,
            StudentId = studentId,
            InstructorId = instructorId,
            Text = text!.Trim(),
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
        };
    }

    public Result Rewrite(string? text, Guid instructorId, DateTimeOffset updatedAt)
    {
        var validation = Validate(text);
        if (validation.IsFailure)
        {
            return validation;
        }

        Text = text!.Trim();
        InstructorId = instructorId;
        UpdatedAt = updatedAt;
        return Result.Success();
    }

    private static Result Validate(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Result.Validation(TextRequiredMessage, SessionErrorCodes.FeedbackRequired, nameof(Text));
        }

        return text.Trim().Length > TextMaxLength
            ? Result.Validation(TextLengthMessage, fieldName: nameof(Text))
            : Result.Success();
    }

    public void Delete(DateTimeOffset deletedOn)
    {
        DeletedOn = DeletedOnGuard.Delete(DeletedOn, deletedOn);
        IsDeleted = true;
    }
}
