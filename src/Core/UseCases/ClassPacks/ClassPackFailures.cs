using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.UseCases.ClassPacks;

internal static class ClassPackFailures
{
    private const string NotFoundMessage = "The class pack does not exist.";
    private const string NameTakenMessage = "Another class pack already has this name.";
    private const string PurchaseNotFoundMessage = "The class pack sale does not exist.";
    private const string PackRequiredMessage = "Choose a class pack.";
    private const string ClassGroupNotFoundMessage = "One of the classes does not exist.";

    public static ResultError NotFound() => new(ClassPackErrorCodes.NotFound, NotFoundMessage, ErrorKind.NotFound);

    public static ResultError NameTaken() => new(ClassPackErrorCodes.NameTaken, NameTakenMessage, ErrorKind.Conflict);

    public static ResultError PurchaseNotFound() => new(ClassPackErrorCodes.PurchaseNotFound, PurchaseNotFoundMessage, ErrorKind.NotFound);

    public static ResultError ClassGroupNotFound(string fieldName) =>
        new(ClassPackErrorCodes.ClassGroupNotFound, ClassGroupNotFoundMessage, ErrorKind.Validation) { FieldName = fieldName };

    public static ResultError PackRequired(string fieldName) =>
        new(ClassPackErrorCodes.NotFound, PackRequiredMessage, ErrorKind.Validation) { FieldName = fieldName };
}
