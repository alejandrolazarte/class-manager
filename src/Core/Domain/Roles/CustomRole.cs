using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;
using PermissionCatalog = ClassManager.Core.Domain.Authorization.Permissions;

namespace ClassManager.Core.Domain.Roles;

public sealed class CustomRole : ITenantOwned, ISoftDeletable
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 60;
    public const int CopiedFromMaxLength = 60;
    public const int PermissionsMaxLength = 2000;

    private const string NameLengthMessage = "The name must be between 2 and 60 characters.";
    private const string CopiedFromLengthMessage = "The original role name must be at most 60 characters.";
    private const string NameReservedMessage = "This name belongs to a role of the app. Choose another one.";
    private const string PermissionsRequiredMessage = "Choose at least one permission.";
    private const string PermissionNotAssignableMessage = "This permission can't be given to a custom role.";

    private static readonly string[] ReservedNames =
    [
        "BrandOwner",
        "BranchOwner",
        "Instructor",
        "Viewer",
        "Custom",
        "Dueño de marca",
        "Dueño de la sede",
        "Solo lectura",
        "Profe",
    ];

    private CustomRole()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public IReadOnlyList<string> Permissions { get; private set; } = [];
    public string? CopiedFrom { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }

    public static Result<CustomRole> Create(string? name, IEnumerable<string>? permissions, string? copiedFrom, DateTimeOffset createdAt)
    {
        var trimmedCopiedFrom = string.IsNullOrWhiteSpace(copiedFrom) ? null : copiedFrom.Trim();
        if (trimmedCopiedFrom?.Length > CopiedFromMaxLength)
        {
            return Result.Validation<CustomRole>(CopiedFromLengthMessage, fieldName: nameof(CopiedFrom));
        }

        var role = new CustomRole
        {
            Id = Guid.CreateVersion7(),
            CopiedFrom = trimmedCopiedFrom,
            CreatedAt = createdAt.ToUniversalTime(),
        };
        var update = role.Update(name, permissions);
        return update.IsFailure ? update.Error! : role;
    }

    public Result Update(string? name, IEnumerable<string>? permissions)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedName.Length is < NameMinLength or > NameMaxLength)
        {
            return Result.Validation(NameLengthMessage, fieldName: nameof(Name));
        }

        if (ReservedNames.Contains(trimmedName, StringComparer.OrdinalIgnoreCase))
        {
            return Result.Validation(NameReservedMessage, RoleErrorCodes.NameReserved, nameof(Name));
        }

        var distinctPermissions = (permissions ?? []).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToList();
        if (distinctPermissions.Count == 0)
        {
            return Result.Validation(PermissionsRequiredMessage, RoleErrorCodes.PermissionsRequired, nameof(Permissions));
        }

        if (distinctPermissions.FirstOrDefault(permission => !PermissionCatalog.Assignable.Contains(permission)) is { } notAssignable)
        {
            return Result.Failure(new ResultError(RoleErrorCodes.PermissionNotAssignable, PermissionNotAssignableMessage, ErrorKind.Validation)
            {
                FieldName = nameof(Permissions),
                Details = new Dictionary<string, object?> { [RoleErrorCodes.PermissionDetail] = notAssignable },
            });
        }

        Name = trimmedName;
        Permissions = distinctPermissions;
        return Result.Success();
    }

    public void Delete(DateTimeOffset deletedOn)
    {
        DeletedOn = DeletedOnGuard.Delete(DeletedOn, deletedOn);
        IsDeleted = true;
    }
}
