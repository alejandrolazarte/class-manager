import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { StudentFields } from "@/features/students/components/StudentFields";
import { studentErrorCodes } from "@/features/students/studentErrorCodes";
import {
  emptyStudentFormValues,
  isEmailOfAnotherPerson,
  StudentFormValues,
  studentFormSchema,
  toNewStudentRequest,
} from "@/features/students/studentSchema";
import { NewStudentRequest } from "@/features/students/types";
import { useAddStudent } from "@/features/students/useAddStudent";
import { SamePersonDialog } from "@/features/users/components/SamePersonDialog";
import { SamePerson, samePersonOf } from "@/features/users/samePerson";
import { userErrorCodes } from "@/features/users/userErrorCodes";
import { translate } from "@/i18n/translate";
import { clientTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { useToast } from "@/ui/ToastProvider";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { useClient } from "@/features/clients/useClient";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { HairlineSection } from "@/ui/HairlineSection";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";

const badRequestStatus = 400;
const studentFieldNames = ["fullName", "birthDate", "notes", "email"] as const;

type SubmissionFailure = "network" | "unexpected";

interface AddStudentScreenProps {
  clientId: string;
}

export function AddStudentScreen({ clientId }: AddStudentScreenProps) {
  const router = useRouter();
  const clientTab = useCurrentTab(clientTabs);
  const { showToast } = useToast();
  const addStudentMutation = useAddStudent(clientId);
  const { data: client } = useClient(clientId);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [samePerson, setSamePerson] = useState<SamePerson | null>(null);
  const form = useForm<StudentFormValues>({
    resolver: zodResolver(studentFormSchema),
    defaultValues: emptyStudentFormValues,
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["fullName"]);

  const handleAddError = (addError: unknown) => {
    if (isNetworkError(addError)) {
      setSubmissionFailure("network");
      return;
    }
    if (isApiError(addError) && addError.hasCode(studentErrorCodes.alreadyRegistered)) {
      form.setError("fullName", {
        type: "server",
        message: translate("students.add.alreadyRegistered"),
      });
      return;
    }
    if (isApiError(addError) && addError.hasCode(studentErrorCodes.emailOfAnotherPerson)) {
      setEmailOfAnotherPersonError();
      return;
    }
    if (
      isApiError(addError) &&
      addError.status === badRequestStatus &&
      applyServerFieldErrors(form, addError.problem, studentFieldNames)
    ) {
      return;
    }
    setSubmissionFailure("unexpected");
  };

  const setEmailOfAnotherPersonError = () =>
    form.setError("email", {
      type: "validate",
      message: translate("students.validation.emailOfAnotherPerson"),
    });

  const submit = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    const familyEmails = [
      client?.email,
      ...(client?.students ?? []).map((student) => student.email),
    ];
    if (isEmailOfAnotherPerson(formValues.email, familyEmails)) {
      setEmailOfAnotherPersonError();
      return;
    }
    await addStudent(toNewStudentRequest(formValues));
  });

  const addStudent = async (request: NewStudentRequest) => {
    try {
      await addStudentMutation.mutateAsync(request);
      setSamePerson(null);
      showToast(translate("students.add.success"));
      router.replace(routes.clientDetail(clientTab, clientId));
    } catch (addError) {
      setSamePerson(samePersonOf(addError, request.email ?? ""));
      if (isApiError(addError) && addError.hasCode(userErrorCodes.samePerson)) {
        return;
      }
      handleAddError(addError);
    }
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="close"
          eyebrow={client?.fullName}
          title={translate("students.add.title")}
        />
      }
    >
      {submissionFailure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={submit} />
        </Banner>
      ) : null}
      {submissionFailure === "unexpected" ? (
        <Banner message={translate("common.unexpectedError")} />
      ) : null}
      {samePerson ? (
        <SamePersonDialog
          person={samePerson}
          messageKey="people.samePerson.inTeam"
          onCancel={() => {
            setSamePerson(null);
            form.setError("email", {
              type: "server",
              message: translate("apiErrors.user.email_taken"),
            });
          }}
          onConfirm={() =>
            void addStudent({
              ...toNewStudentRequest(form.getValues()),
              samePersonUserId: samePerson.userId,
            })
          }
          isConfirming={addStudentMutation.isPending}
        />
      ) : null}
      <RequiredFieldsLegend />
      <HairlineSection title={translate("students.add.studentCard")}>
        <StudentFields
          control={form.control}
          paths={{ fullName: "fullName", birthDate: "birthDate", notes: "notes", email: "email" }}
          autoFocus
        />
      </HairlineSection>
      <Button
        label={translate("students.add.submit")}
        onPress={submit}
        disabled={!areRequiredFieldsFilled}
        isLoading={addStudentMutation.isPending}
      />
    </ScrollScreen>
  );
}
