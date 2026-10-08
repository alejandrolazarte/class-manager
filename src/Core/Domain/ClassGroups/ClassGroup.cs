using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassGroups;

public sealed class ClassGroup : ITenantOwned
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 80;
    public const int MinimumCapacity = 1;
    public const int MaximumCapacity = 100;
    public const int LocationMaxLength = 80;
    public const int MaterialUrlMaxLength = 500;

    private const string NameLengthMessage = "Name must be between 2 and 80 characters.";
    private const string CapacityRangeMessage = "Capacity must be between 1 and 100.";
    private const string LocationLengthMessage = "Location must be at most 80 characters.";
    private const string MaterialUrlMessage = "The material link must be an https address of at most 500 characters.";

    private ClassGroup()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid InstructorId { get; private set; }
    public ClassWeekdays Weekdays { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public int DurationMinutes { get; private set; }
    public int Capacity { get; private set; }
    public string? Location { get; private set; }
    public string? MaterialUrl { get; private set; }
    public Guid? MaterialDocumentId { get; private set; }
    public Document? MaterialDocument { get; private set; }
    public bool IsActive { get; private set; }

    public ClassSchedule Schedule => ClassSchedule.FromStored(Weekdays, StartTime, DurationMinutes);

    public static Result<ClassGroup> Create(string? name, Guid instructorId, ClassSchedule schedule, int? capacity, string? location)
    {
        var classGroup = new ClassGroup { Id = Guid.CreateVersion7(), IsActive = true };
        var update = classGroup.Update(name, instructorId, schedule, capacity, location);

        return update.IsFailure ? update.Error! : classGroup;
    }

    public Result Update(string? name, Guid instructorId, ClassSchedule schedule, int? capacity, string? location)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedName.Length is < NameMinLength or > NameMaxLength)
        {
            return Result.Validation(NameLengthMessage, fieldName: nameof(Name));
        }

        if (capacity is not (>= MinimumCapacity and <= MaximumCapacity))
        {
            return Result.Validation(CapacityRangeMessage, fieldName: nameof(Capacity));
        }

        var trimmedLocation = string.IsNullOrWhiteSpace(location) ? null : location.Trim();
        if (trimmedLocation?.Length > LocationMaxLength)
        {
            return Result.Validation(LocationLengthMessage, fieldName: nameof(Location));
        }

        Name = trimmedName;
        InstructorId = instructorId;
        Weekdays = schedule.Weekdays;
        StartTime = schedule.StartTime;
        DurationMinutes = schedule.DurationMinutes;
        Capacity = capacity.Value;
        Location = trimmedLocation;
        return Result.Success();
    }

    public Result ShareMaterial(string? materialUrl)
    {
        var trimmedUrl = string.IsNullOrWhiteSpace(materialUrl) ? null : materialUrl.Trim();
        if (trimmedUrl is not null
            && (trimmedUrl.Length > MaterialUrlMaxLength
                || !Uri.TryCreate(trimmedUrl, UriKind.Absolute, out var parsedUrl)
                || parsedUrl.Scheme != Uri.UriSchemeHttps))
        {
            return Result.Validation(MaterialUrlMessage, fieldName: nameof(MaterialUrl));
        }

        MaterialUrl = trimmedUrl;
        if (trimmedUrl is not null)
        {
            RemoveMaterialFile();
        }

        return Result.Success();
    }

    public Document? ShareMaterialFile(Document file)
    {
        var replacedFile = RemoveMaterialFile();
        MaterialUrl = null;
        MaterialDocumentId = file.Id;
        MaterialDocument = file;
        return replacedFile;
    }

    public Document? RemoveMaterialFile()
    {
        var removedFile = MaterialDocument;
        MaterialDocumentId = null;
        MaterialDocument = null;
        return removedFile;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
