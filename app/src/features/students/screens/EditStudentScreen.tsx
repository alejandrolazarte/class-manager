import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { isApiError } from "@/api/httpClient";
import { formatBirthDateForDisplay } from "@/dates/birthDateFormatting";
import { clientErrorCodes } from "@/features/clients/clientErrorCodes";
import { ClientDetails } from "@/features/clients/types";
import { useClient } from "@/features/clients/useClient";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { StudentFields } from "@/features/students/components/StudentFields";
import { studentErrorCodes } from "@/features/students/studentErrorCodes";
import {
  isEmailOfAnotherPerson,
  normalizeStudentName,
  StudentFormValues,
  studentFormSchema,
  toNewStudentRequest,
} from "@/features/students/studentSchema";
import { Student } from "@/features/students/types";
import { useUpdateStudent } from "@/features/students/useUpdateStudent";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonScreen } from "@/ui/SkeletonScreen";
import { useToast } from "@/ui/ToastProvider";

const badRequestStatus = 400;
const studentFieldNames = ["fullName", "birthDate", "notes", "email"] as const;

interface EditStudentScreenProps {
  clientId: string;
  studentId: string;
}

interface StudentEditorProps {
  client: ClientDetails;
  student: Student;
}

function toStudentFormValues(student: Student): StudentFormValues {
  return {
    fullName: student.fullName,
    birthDate: student.birthDate ? formatBirthDateForDisplay(student.birthDate) : "",
    notes: student.notes ?? "",
    email: student.appAccess.signInEmail ?? student.email ?? "",
  };
}

function StudentEditor({ client, student }: StudentEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const updateStudentMutation = useUpdateStudent(student.id);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const form = useForm<StudentFormValues>({
    resolver: zodResolver(studentFormSchema),
    defaultValues: toStudentFormValues(student),
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["fullName"]);
  const isAttendingContact =
    normalizeStudentName(student.fullName) === normalizeStudentName(client.fullName);
  const isInvited = student.appAccess.status === "Invited";
  const emailHints = {
    NotInvited: translate("students.fields.emailHint"),
    Invited: translate("clients.edit.emailHintInvited"),
    Active: translate("clients.edit.emailHintActive"),
    AwaitingGuardianConsent: translate("students.fields.emailHint"),
  };

  const resendInvitationIfEmailChanged = async (email: string | null) => {
    if (!isInvited || email === null || email === student.appAccess.invitedEmail) {
      return;
    }
    try {
      await inviteStudentApp(client.id, { email, studentId: student.id });
    } catch {
      showToast(translate("clients.register.invitationFailed"));
    }
  };

  const setEmailOfAnotherPersonError = () =>
    form.setError("email", {
      type: "validate",
      message: translate("students.validation.emailOfAnotherPerson"),
    });

  const handleSaveError = (saveError: unknown) => {
    if (isApiError(saveError) && saveError.hasCode(clientErrorCodes.emailUsedToSignIn)) {
      form.setError("email", {
        type: "server",
        message: translate("clients.edit.emailHintActive"),
      });
      return;
    }
    if (isApiError(saveError) && saveError.hasCode(clientErrorCodes.contactMustBeAdult)) {
      form.setError("birthDate", {
        type: "server",
        message: translate("clients.validation.contactMustBeAdult", { name: client.fullName }),
      });
      return;
    }
    if (isApiError(saveError) && saveError.hasCode(studentErrorCodes.alreadyRegistered)) {
      form.setError("fullName", {
        type: "server",
        message: translate("students.add.alreadyRegistered"),
      });
      return;
    }
    if (isApiError(saveError) && saveError.hasCode(studentErrorCodes.emailOfAnotherPerson)) {
      setEmailOfAnotherPersonError();
      return;
    }
    if (
      isApiError(saveError) &&
      saveError.status === badRequestStatus &&
      applyServerFieldErrors(form, saveError.problem, studentFieldNames)
    ) {
      return;
    }
    setSubmissionFailure(toSubmissionFailure(saveError));
  };

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    const familyEmails = [
      client.email,
      ...client.students.filter((other) => other.id !== student.id).map((other) => other.email),
    ];
    if (!isAttendingContact && isEmailOfAnotherPerson(formValues.email, familyEmails)) {
      setEmailOfAnotherPersonError();
      return;
    }
    const request = toNewStudentRequest(formValues);
    try {
      await updateStudentMutation.mutateAsync(request);
    } catch (saveError) {
      handleSaveError(saveError);
      return;
    }
    await resendInvitationIfEmailChanged(request.email);
    showToast(translate("students.edit.saved"));
    router.back();
  });

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="close"
          eyebrow={student.fullName}
          title={translate("students.edit.title")}
        />
      }
      footer={
        <ScreenFooter>
          <Button
            label={translate("students.edit.submit")}
            onPress={save}
            disabled={!areRequiredFieldsFilled}
            isLoading={form.formState.isSubmitting}
          />
        </ScreenFooter>
      }
    >
      {submissionFailure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={save} />
        </Banner>
      ) : null}
      {submissionFailure === "unexpected" ? (
        <Banner message={translate("common.unexpectedError")} />
      ) : null}
      {isAttendingContact ? null : <RequiredFieldsLegend />}
      <StudentFields
        control={form.control}
        paths={{ fullName: "fullName", birthDate: "birthDate", notes: "notes", email: "email" }}
        emailHint={emailHints[student.appAccess.status]}
        isEmailEditable={student.appAccess.status !== "Active"}
        isAttendingContact={isAttendingContact}
      />
    </ScrollScreen>
  );
}

export function EditStudentScreen({ clientId, studentId }: EditStudentScreenProps) {
  const { data: client } = useClient(clientId);
  const student = client?.students.find((clientStudent) => clientStudent.id === studentId);
  if (client === undefined || student === undefined) {
    return <SkeletonScreen />;
  }
  return <StudentEditor client={client} student={student} />;
}
