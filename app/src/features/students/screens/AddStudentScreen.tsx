import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { KeyboardAvoidingView, Platform, ScrollView } from "react-native";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { StudentFields } from "@/features/students/components/StudentFields";
import { studentErrorCodes } from "@/features/students/studentErrorCodes";
import {
  emptyStudentFormValues,
  StudentFormValues,
  studentFormSchema,
  toNewStudentRequest,
} from "@/features/students/studentSchema";
import { useAddStudent } from "@/features/students/useAddStudent";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { useToast } from "@/ui/ToastProvider";

const badRequestStatus = 400;
const studentFieldNames = ["fullName", "birthDate", "notes"] as const;

type SubmissionFailure = "network" | "unexpected";

interface AddStudentScreenProps {
  clientId: string;
}

export function AddStudentScreen({ clientId }: AddStudentScreenProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const addStudentMutation = useAddStudent(clientId);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const form = useForm<StudentFormValues>({
    resolver: zodResolver(studentFormSchema),
    defaultValues: emptyStudentFormValues,
    mode: "onTouched",
  });

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
    if (
      isApiError(addError) &&
      addError.status === badRequestStatus &&
      applyServerFieldErrors(form, addError.problem, studentFieldNames)
    ) {
      return;
    }
    setSubmissionFailure("unexpected");
  };

  const submit = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    try {
      await addStudentMutation.mutateAsync(toNewStudentRequest(formValues));
      showToast(translate("students.add.success"));
      router.replace(routes.clientDetail(clientId));
    } catch (addError) {
      handleAddError(addError);
    }
  });

  return (
    <KeyboardAvoidingView
      className="flex-1 bg-gray-50"
      behavior={Platform.OS === "ios" ? "padding" : undefined}
    >
      <ScrollView contentContainerClassName="gap-4 p-4" keyboardShouldPersistTaps="handled">
        {submissionFailure === "network" ? (
          <Banner message={translate("common.networkError")}>
            <Button variant="secondary" label={translate("common.retry")} onPress={submit} />
          </Banner>
        ) : null}
        {submissionFailure === "unexpected" ? (
          <Banner message={translate("common.unexpectedError")} />
        ) : null}
        <StudentFields
          control={form.control}
          paths={{ fullName: "fullName", birthDate: "birthDate", notes: "notes" }}
          autoFocus
        />
        <Button
          label={translate("students.add.submit")}
          onPress={submit}
          isLoading={addStudentMutation.isPending}
        />
      </ScrollView>
    </KeyboardAvoidingView>
  );
}
