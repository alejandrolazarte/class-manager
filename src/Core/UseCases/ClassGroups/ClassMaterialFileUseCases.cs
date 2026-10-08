using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record UploadClassMaterialFileCommand(Guid ClassGroupId, byte[] Content) : ICommand;

public sealed record RemoveClassMaterialFileCommand(Guid ClassGroupId) : ICommand;

public sealed class UploadClassMaterialFileUseCase(
    IClassGroupRepository classGroupRepository,
    IDocumentRepository documentRepository,
    IDocumentStorageService documentStorage,
    IUnitOfWork unitOfWork,
    IInstructorRepository instructorRepository,
    IEnrollmentRepository enrollmentRepository,
    IBusinessCalendarService businessCalendar)
    : IUseCase<UploadClassMaterialFileCommand, ClassGroupResponse>
{
    public async Task<Result<ClassGroupResponse>> ExecuteAsync(UploadClassMaterialFileCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetForUpdateAsync(command.ClassGroupId, cancellationToken);
        if (classGroup is null)
        {
            return ClassGroupRules.NotFound();
        }

        var content = ClassMaterialFile.Validate(command.Content);
        if (content.IsFailure)
        {
            return content.Error!;
        }

        var file = await documentStorage.StoreAsync(DocumentOwner.ClassGroup, classGroup.Id, content.Value!, DocumentVisibility.Public, cancellationToken);
        documentRepository.Add(file);
        var replacedFile = classGroup.ShareMaterialFile(file);
        if (replacedFile is not null)
        {
            documentRepository.Remove(replacedFile);
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await documentStorage.DeleteFileAsync(file, CancellationToken.None);
            throw;
        }

        if (replacedFile is not null)
        {
            await documentStorage.DeleteFileAsync(replacedFile, cancellationToken);
        }

        return await ClassMaterialFileEditing.ResponseAsync(
            classGroup, instructorRepository, enrollmentRepository, businessCalendar, documentStorage, cancellationToken);
    }
}

public sealed class RemoveClassMaterialFileUseCase(
    IClassGroupRepository classGroupRepository,
    IDocumentRepository documentRepository,
    IDocumentStorageService documentStorage,
    IUnitOfWork unitOfWork,
    IInstructorRepository instructorRepository,
    IEnrollmentRepository enrollmentRepository,
    IBusinessCalendarService businessCalendar)
    : IUseCase<RemoveClassMaterialFileCommand, ClassGroupResponse>
{
    private const string FileNotFoundMessage = "The class has no material file.";

    public async Task<Result<ClassGroupResponse>> ExecuteAsync(RemoveClassMaterialFileCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetForUpdateAsync(command.ClassGroupId, cancellationToken);
        if (classGroup is null)
        {
            return ClassGroupRules.NotFound();
        }

        var removedFile = classGroup.RemoveMaterialFile();
        if (removedFile is null)
        {
            return Result.NotFound<ClassGroupResponse>(FileNotFoundMessage, ClassGroupErrorCodes.MaterialFileNotFound);
        }

        documentRepository.Remove(removedFile);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await documentStorage.DeleteFileAsync(removedFile, cancellationToken);

        return await ClassMaterialFileEditing.ResponseAsync(
            classGroup, instructorRepository, enrollmentRepository, businessCalendar, documentStorage, cancellationToken);
    }
}

internal static class ClassMaterialFileEditing
{
    public static async Task<ClassGroupResponse> ResponseAsync(
        ClassGroup classGroup,
        IInstructorRepository instructorRepository,
        IEnrollmentRepository enrollmentRepository,
        IBusinessCalendarService businessCalendar,
        IDocumentStorageService documentStorage,
        CancellationToken cancellationToken)
    {
        var instructor = await instructorRepository.GetByIdAsync(classGroup.InstructorId, cancellationToken);
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var enrolledCount = await enrollmentRepository.CountCurrentAsync(classGroup.Id, today, cancellationToken);
        return ClassGroupResponse.From(classGroup, instructor?.FullName ?? string.Empty, enrolledCount, documentStorage);
    }
}
