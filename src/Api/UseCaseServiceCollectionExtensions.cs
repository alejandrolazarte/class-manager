using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Services;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Businesses;
using ClassManager.Core.UseCases.ClassGroups;
using ClassManager.Core.UseCases.ClassPacks;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Enrollments;
using ClassManager.Core.UseCases.Fees;
using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.Core.UseCases.ImportExport.Students;
using ClassManager.Core.UseCases.Instructors;
using ClassManager.Core.UseCases.Members;
using ClassManager.Core.UseCases.PrivateLessons;
using ClassManager.Core.UseCases.Sessions;
using ClassManager.Core.UseCases.Students;
using ClassManager.ImportExport.Parsing;
using ClassManager.ImportExport.Tabular;
using ClassManager.ImportExport.Tabular.Csv;
using ClassManager.ImportExport.Xlsx;

namespace ClassManager.Api;

internal static class UseCaseServiceCollectionExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IBusinessCalendarService, BusinessCalendarService>();
        services.AddScoped<IClassBalanceService, ClassBalanceService>();
        services.AddSingleton<CsvTabularReader>();
        services.AddSingleton(new XlsxTabularReader());
        services.AddSingleton<ITabularReader, XlsxOrCsvTabularReader>();
        services.AddSingleton<ITabularWriter, XlsxTabularWriter>();
        services.AddSingleton<IImportParser, ImportParser>();
        services.AddScoped<IImportModule, InstructorImportModule>();
        services.AddScoped<IImportModule, StudentImportModule>();

        services.AddScoped<IUseCase<SignUpOwnerCommand, TokenResponse>, SignUpOwnerUseCase>();
        services.AddScoped<IUseCase<GetImportSchemaQuery, ImportSchemaResponse>, GetImportSchemaUseCase>();
        services.AddScoped<IUseCase<PreviewImportCommand, ImportReport>, PreviewImportUseCase>();
        services.AddScoped<IUseCase<ImportFileCommand, ImportReport>, ImportFileUseCase>();
        services.AddScoped<IUseCase<GetImportTemplateQuery, ExportFile>, GetImportTemplateUseCase>();
        services.AddScoped<IUseCase<ExportQuery, ExportFile>, ExportUseCase>();
        services.AddScoped<IUseCase<SignInCommand, TokenResponse>, SignInUseCase>();
        services.AddScoped<IUseCase<RefreshSessionCommand, TokenResponse>, RefreshSessionUseCase>();
        services.AddScoped<IUseCase<SignOutCommand, SignOutResponse>, SignOutUseCase>();
        services.AddScoped<IUseCase<ResetPasswordCommand, ResetPasswordResponse>, ResetPasswordUseCase>();
        services.AddScoped<IUseCase<RequestPasswordResetCommand, RequestPasswordResetResponse>, RequestPasswordResetUseCase>();
        services.AddScoped<IUseCase<GetCurrentBusinessQuery, BusinessResponse>, GetCurrentBusinessUseCase>();
        services.AddScoped<IUseCase<GetCurrentMemberQuery, CurrentMemberResponse>, GetCurrentMemberUseCase>();
        services.AddScoped<IUseCase<RegisterClientCommand, ClientDetailsResponse>, RegisterClientUseCase>();
        services.AddScoped<IUseCase<GetClientQuery, ClientDetailsResponse>, GetClientUseCase>();
        services.AddScoped<IUseCase<SearchClientsQuery, IReadOnlyList<ClientResponse>>, SearchClientsUseCase>();
        services.AddScoped<IUseCase<AddStudentCommand, StudentResponse>, AddStudentUseCase>();
        services.AddScoped<IUseCase<GetStudentQuery, StudentSummaryResponse>, GetStudentUseCase>();
        services.AddScoped<IUseCase<SearchStudentsQuery, IReadOnlyList<StudentSummaryResponse>>, SearchStudentsUseCase>();
        services.AddScoped<IUseCase<ListInstructorsQuery, IReadOnlyList<InstructorResponse>>, ListInstructorsUseCase>();
        services.AddScoped<IUseCase<CreateInstructorCommand, InstructorResponse>, CreateInstructorUseCase>();
        services.AddScoped<IUseCase<UpdateInstructorCommand, InstructorResponse>, UpdateInstructorUseCase>();
        services.AddScoped<IUseCase<SetInstructorActiveCommand, InstructorResponse>, SetInstructorActiveUseCase>();
        services.AddScoped<IUseCase<ListClassGroupsQuery, IReadOnlyList<ClassGroupResponse>>, ListClassGroupsUseCase>();
        services.AddScoped<IUseCase<GetClassGroupQuery, ClassGroupResponse>, GetClassGroupUseCase>();
        services.AddScoped<IUseCase<CreateClassGroupCommand, ClassGroupResponse>, CreateClassGroupUseCase>();
        services.AddScoped<IUseCase<UpdateClassGroupCommand, ClassGroupResponse>, UpdateClassGroupUseCase>();
        services.AddScoped<IUseCase<SetClassGroupActiveCommand, ClassGroupResponse>, SetClassGroupActiveUseCase>();
        services.AddScoped<IUseCase<EnrollStudentCommand, EnrollmentResponse>, EnrollStudentUseCase>();
        services.AddScoped<IUseCase<EndEnrollmentCommand, EndEnrollmentResponse>, EndEnrollmentUseCase>();
        services.AddScoped<IUseCase<ListClassRosterQuery, IReadOnlyList<RosterEntryResponse>>, ListClassRosterUseCase>();
        services.AddScoped<IUseCase<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentResponse>>, ListStudentEnrollmentsUseCase>();
        services.AddScoped<IUseCase<ListDaySessionsQuery, IReadOnlyList<DaySessionResponse>>, ListDaySessionsUseCase>();
        services.AddScoped<IUseCase<ListMonthCalendarQuery, MonthCalendarResponse>, ListMonthCalendarUseCase>();
        services.AddScoped<IUseCase<SchedulePrivateLessonCommand, IReadOnlyList<PrivateLessonResponse>>, SchedulePrivateLessonUseCase>();
        services.AddScoped<IUseCase<GetPrivateLessonQuery, PrivateLessonResponse>, GetPrivateLessonUseCase>();
        services.AddScoped<IUseCase<ReschedulePrivateLessonCommand, PrivateLessonResponse>, ReschedulePrivateLessonUseCase>();
        services.AddScoped<IUseCase<CancelPrivateLessonCommand, PrivateLessonResponse>, CancelPrivateLessonUseCase>();
        services.AddScoped<IUseCase<RestorePrivateLessonCommand, PrivateLessonResponse>, RestorePrivateLessonUseCase>();
        services.AddScoped<IUseCase<RecordPrivateLessonAttendanceCommand, RecordAttendanceResponse>, RecordPrivateLessonAttendanceUseCase>();
        services.AddScoped<IUseCase<DeletePrivateLessonCommand, DeletedPrivateLessonResponse>, DeletePrivateLessonUseCase>();
        services.AddScoped<IUseCase<GetSessionQuery, SessionDetailsResponse>, GetSessionUseCase>();
        services.AddScoped<IUseCase<RecordAttendanceCommand, RecordAttendanceResponse>, RecordAttendanceUseCase>();
        services.AddScoped<IUseCase<CancelSessionCommand, SessionStatusResponse>, CancelSessionUseCase>();
        services.AddScoped<IUseCase<RestoreSessionCommand, SessionStatusResponse>, RestoreSessionUseCase>();
        services.AddScoped<IUseCase<RescheduleSessionCommand, SessionStatusResponse>, RescheduleSessionUseCase>();
        services.AddScoped<IUseCase<RestoreSessionScheduleCommand, SessionStatusResponse>, RestoreSessionScheduleUseCase>();
        services.AddScoped<IUseCase<SetDefaultMonthlyFeeCommand, BusinessResponse>, SetDefaultMonthlyFeeUseCase>();
        services.AddScoped<IUseCase<SetClientBillingPlanCommand, ClientBillingResponse>, SetClientBillingPlanUseCase>();
        services.AddScoped<IUseCase<ListClassPacksQuery, IReadOnlyList<ClassPackResponse>>, ListClassPacksUseCase>();
        services.AddScoped<IUseCase<CreateClassPackCommand, ClassPackResponse>, CreateClassPackUseCase>();
        services.AddScoped<IUseCase<UpdateClassPackCommand, ClassPackResponse>, UpdateClassPackUseCase>();
        services.AddScoped<IUseCase<SetClassPackActiveCommand, ClassPackResponse>, SetClassPackActiveUseCase>();
        services.AddScoped<IUseCase<SellClassPackCommand, ClassPackPurchaseResponse>, SellClassPackUseCase>();
        services.AddScoped<IUseCase<DeleteClassPackPurchaseCommand, ClassPackPurchaseResponse>, DeleteClassPackPurchaseUseCase>();
        services.AddScoped<IUseCase<GetClientClassBalanceQuery, ClassBalanceResponse>, GetClientClassBalanceUseCase>();
        services.AddScoped<IUseCase<RecordPaymentCommand, PaymentResponse>, RecordPaymentUseCase>();
        services.AddScoped<IUseCase<ListClientPaymentsQuery, IReadOnlyList<PaymentResponse>>, ListClientPaymentsUseCase>();
        services.AddScoped<IUseCase<DeletePaymentCommand, PaymentResponse>, DeletePaymentUseCase>();
        services.AddScoped<IUseCase<ListMonthlyFeesQuery, MonthlyFeesResponse>, ListMonthlyFeesUseCase>();
        services.AddScoped<IUseCase<UpdateBusinessSettingsCommand, UpdateBusinessSettingsResponse>, UpdateBusinessSettingsUseCase>();

        return services;
    }
}
