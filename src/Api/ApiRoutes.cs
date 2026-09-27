namespace ClassManager.Api;

public static class ApiRoutes
{
    public const string Health = "/health";
    public const string Clients = "/api/clients";
    public const string ClientById = "/{clientId:guid}";
    public const string StudentsOfClient = "/students";
    public const string Students = "/api/students";
    public const string StudentById = "/{studentId:guid}";
    public const string Instructors = "/api/instructors";
    public const string InstructorById = "/{instructorId:guid}";
    public const string ClassGroups = "/api/class-groups";
    public const string ClassGroupById = "/{classGroupId:guid}";
    public const string Active = "/active";
    public const string EnrollmentsSegment = "/enrollments";
    public const string Enrollments = "/api/enrollments";
    public const string EnrollmentById = "/{enrollmentId:guid}";
    public const string End = "/end";
    public const string Sessions = "/api/sessions";
    public const string SessionsSegment = "/sessions";
    public const string Calendar = "/calendar";
    public const string SessionByDate = "/{sessionDate}";
    public const string Attendance = "/attendance";
    public const string AttendanceByStudent = "/attendance/{studentId:guid}";
    public const string Cancellation = "/cancellation";
    public const string Schedule = "/schedule";
    public const string MonthlyFee = "/monthly-fee";
    public const string BillingPlan = "/billing-plan";
    public const string ClassPacks = "/api/class-packs";
    public const string ClassPackById = "/{classPackId:guid}";
    public const string ClassPackPurchasesSegment = "/class-pack-purchases";
    public const string ClassPackPurchases = "/api/class-pack-purchases";
    public const string ClassPackPurchaseById = "/{purchaseId:guid}";
    public const string ClassBalance = "/class-balance";
    public const string PaymentsSegment = "/payments";
    public const string Payments = "/api/payments";
    public const string PaymentById = "/{paymentId:guid}";
    public const string Fees = "/api/fees";
    public const string PrivateLessons = "/api/private-lessons";
    public const string PrivateLessonById = "/{privateLessonId:guid}";
    public const string Business = "/api/business";
    public const string Authentication = "/api/auth";
    public const string SignUp = "/sign-up";
    public const string SignIn = "/sign-in";
    public const string Refresh = "/refresh";
    public const string SignOut = "/sign-out";
    public const string PasswordReset = "/password-reset";
}
