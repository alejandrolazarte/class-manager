using System.Globalization;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record ClassMaterialFileResponse(long Id, string Url, long SizeInBytes);

public sealed record ClassGroupResponse(
    long Id,
    string Name,
    long InstructorId,
    string InstructorFullName,
    IReadOnlyList<DayOfWeek> Weekdays,
    string StartTime,
    string EndTime,
    int DurationMinutes,
    int Capacity,
    string? Location,
    string? MaterialUrl,
    ClassMaterialFileResponse? MaterialFile,
    bool IsActive,
    int EnrolledCount,
    string? InstructorPhotoUrl = null)
{
    public static ClassGroupResponse From(
        ClassGroup classGroup, string instructorFullName, int enrolledCount, IDocumentStorageService documentStorage)
    {
        var schedule = classGroup.Schedule;
        return new(
            classGroup.Id,
            classGroup.Name,
            classGroup.InstructorId,
            instructorFullName,
            schedule.Days,
            schedule.StartTime.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture),
            schedule.EndTime.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture),
            schedule.DurationMinutes,
            classGroup.Capacity,
            classGroup.Location,
            classGroup.MaterialUrl,
            classGroup.MaterialFile is { } materialFile
                ? new ClassMaterialFileResponse(materialFile.Id, documentStorage.PublicUrlOf(materialFile).ToString(), materialFile.SizeInBytes)
                : null,
            classGroup.IsActive,
            enrolledCount);
    }
}
