using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Sessions;

public sealed class ClassFeedback : ITenantOwned, IAuditable, ISoftDeletable
{
    public const int TextMaxLength = 500;

    private const string TextRequiredMessage = "Write the comment.";
    private const string TextLengthMessage = "The comment must be at most 500 characters.";

    private ClassFeedback()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long ClassSessionId { get; private set; }
    public ClassSession? Session { get; private set; }
    public long StudentId { get; private set; }
    public long InstructorId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Result<ClassFeedback> Create(
        ClassSession session, long studentId, long instructorId, string? text, DateTimeOffset createdAt)
    {
        var validation = Validate(text);
        if (validation.IsFailure)
        {
            return validation.Error!;
        }

        return new ClassFeedback
        {
            ClassSessionId = session.Id,
            Session = session,
            StudentId = studentId,
            InstructorId = instructorId,
            Text = text!.Trim(),
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
        };
    }

    public Result Rewrite(string? text, long instructorId, DateTimeOffset updatedAt)
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

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
