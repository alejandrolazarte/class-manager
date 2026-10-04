import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { View } from "react-native";
import { getNumberExtension } from "@/api/problemDetails";
import { isApiError } from "@/api/httpClient";
import {
  classGroupCountExtension,
  instructorErrorCodes,
} from "@/features/instructors/instructorErrorCodes";
import {
  instructorFieldNames,
  InstructorFormValues,
  instructorSchema,
  toInstructorFormValues,
  toSaveInstructorRequest,
} from "@/features/instructors/instructorSchema";
import { Instructor } from "@/features/instructors/types";
import { useInstructorsIncludingInactive } from "@/features/instructors/useInstructorsIncludingInactive";
import {
  useSaveInstructor,
  useSetInstructorActive,
} from "@/features/instructors/useInstructorMutations";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate, translateCount } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";

const badRequestStatus = 400;

interface InstructorFormScreenProps {
  instructorId?: string;
}

interface InstructorEditorProps {
  instructor?: Instructor;
}

function InstructorEditor({ instructor }: InstructorEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const saveInstructorMutation = useSaveInstructor();
  const setInstructorActiveMutation = useSetInstructorActive();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [activeClassGroupCount, setActiveClassGroupCount] = useState(0);
  const form = useForm<InstructorFormValues>({
    resolver: zodResolver(instructorSchema),
    defaultValues: toInstructorFormValues(instructor),
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["fullName"]);

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    try {
      await saveInstructorMutation.mutateAsync({
        instructorId: instructor?.id,
        request: toSaveInstructorRequest(formValues),
      });
      showToast(translate("instructors.form.saved"));
      router.back();
    } catch (saveError) {
      if (isApiError(saveError) && saveError.hasCode(instructorErrorCodes.nameTaken)) {
        form.setError("fullName", {
          type: "server",
          message: translate("instructors.validation.nameTaken"),
        });
        return;
      }
      const hasFieldErrors =
        isApiError(saveError) &&
        saveError.status === badRequestStatus &&
        applyServerFieldErrors(form, saveError.problem, instructorFieldNames);
      if (!hasFieldErrors) {
        setSubmissionFailure(toSubmissionFailure(saveError));
      }
    }
  });

  const toggleActive = async () => {
    if (instructor === undefined) {
      return;
    }
    setSubmissionFailure(null);
    const isActive = !instructor.isActive;
    try {
      await setInstructorActiveMutation.mutateAsync({ instructorId: instructor.id, isActive });
      showToast(
        translate(isActive ? "instructors.form.activated" : "instructors.form.deactivated"),
      );
      router.back();
    } catch (activationError) {
      if (
        isApiError(activationError) &&
        activationError.hasCode(instructorErrorCodes.hasActiveClassGroups)
      ) {
        setActiveClassGroupCount(
          getNumberExtension(activationError.problem, classGroupCountExtension) ?? 1,
        );
        return;
      }
      setSubmissionFailure(toSubmissionFailure(activationError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate(instructor ? "instructors.form.editTitle" : "instructors.form.newTitle")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      {activeClassGroupCount > 0 ? (
        <Banner
          tone="warning"
          message={translateCount("instructors.form.hasActiveClassGroups", activeClassGroupCount)}
        />
      ) : null}
      <RequiredFieldsLegend />
      <Controller
        control={form.control}
        name="fullName"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("instructors.form.fullName")}
            isRequired
            autoCapitalize="words"
            autoComplete="name"
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <View className="gap-3">
        <Button
          label={translate("common.save")}
          onPress={save}
          disabled={!areRequiredFieldsFilled}
          isLoading={saveInstructorMutation.isPending}
        />
        {instructor ? (
          <Button
            variant={instructor.isActive ? "dangerOutline" : "outline"}
            size="medium"
            label={translate(instructor.isActive ? "common.deactivate" : "common.activate")}
            onPress={toggleActive}
            isLoading={setInstructorActiveMutation.isPending}
          />
        ) : null}
      </View>
    </SettingsFormScreenLayout>
  );
}

export function InstructorFormScreen({ instructorId }: InstructorFormScreenProps) {
  const instructorsQuery = useInstructorsIncludingInactive({ enabled: instructorId !== undefined });
  if (instructorId === undefined) {
    return <InstructorEditor />;
  }
  const instructor = instructorsQuery.data?.find((candidate) => candidate.id === instructorId);
  if (instructor === undefined) {
    return (
      <SettingsItemState
        navigation="close"
        isPending={instructorsQuery.isPending}
        isError={instructorsQuery.isError}
        notFoundMessage={translate("instructors.form.notFound")}
        onRetry={() => instructorsQuery.refetch()}
      />
    );
  }
  return <InstructorEditor key={instructor.id} instructor={instructor} />;
}
