using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

internal static class StudentPlanFailures
{
    private const string PlanNotOfferedMessage = "Choose a monthly plan that is offered.";
    private const string StudentNotFoundMessage = "The student does not exist.";
    private const string ChangeNotFoundMessage = "There is no plan change for that month.";

    public static ResultError PlanNotOffered(string fieldName) =>
        new(StudentPlanErrorCodes.PlanNotOffered, PlanNotOfferedMessage, ErrorKind.Validation) { FieldName = fieldName };

    public static ResultError StudentNotFound() =>
        new(StudentPlanErrorCodes.StudentNotFound, StudentNotFoundMessage, ErrorKind.NotFound);

    public static ResultError ChangeNotFound() =>
        new(StudentPlanErrorCodes.ChangeNotFound, ChangeNotFoundMessage, ErrorKind.NotFound);
}
