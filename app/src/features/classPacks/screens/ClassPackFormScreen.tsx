import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { classPackErrorCodes } from "@/features/classPacks/classPackErrorCodes";
import {
  classPackFieldNames,
  ClassPackFormValues,
  classPackSchema,
  toClassPackFormValues,
  toSaveClassPackRequest,
} from "@/features/classPacks/classPackSchema";
import { ClassPack } from "@/features/classPacks/types";
import {
  useSaveClassPack,
  useSetClassPackActive,
} from "@/features/classPacks/useClassPackMutations";
import { useClassPacks } from "@/features/classPacks/useClassPacks";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate, TranslationKey } from "@/i18n/translate";
import { commonDurationsInMinutes } from "@/features/classGroups/classGroupSchema";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const badRequestStatus = 400;

interface ClassPackFormScreenProps {
  classPackId?: string;
}

interface ClassPackEditorProps {
  classPack?: ClassPack;
}

interface ClassPackFieldOptions {
  name: keyof ClassPackFormValues;
  labelKey: TranslationKey;
  placeholderKey?: TranslationKey;
  keyboardType?: "default" | "number-pad" | "decimal-pad" | "url";
}

function ClassPackEditor({ classPack }: ClassPackEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const saveClassPackMutation = useSaveClassPack();
  const setClassPackActiveMutation = useSetClassPackActive();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const form = useForm<ClassPackFormValues>({
    resolver: zodResolver(classPackSchema),
    defaultValues: toClassPackFormValues(classPack),
    mode: "onTouched",
  });

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    try {
      await saveClassPackMutation.mutateAsync({
        classPackId: classPack?.id,
        request: toSaveClassPackRequest(formValues),
      });
      showToast(translate("classPacks.form.saved"));
      router.back();
    } catch (saveError) {
      if (isApiError(saveError) && saveError.hasCode(classPackErrorCodes.nameTaken)) {
        form.setError("name", {
          type: "server",
          message: translate("classPacks.validation.nameTaken"),
        });
        return;
      }
      const hasFieldErrors =
        isApiError(saveError) &&
        saveError.status === badRequestStatus &&
        applyServerFieldErrors(form, saveError.problem, classPackFieldNames);
      if (!hasFieldErrors) {
        setSubmissionFailure(toSubmissionFailure(saveError));
      }
    }
  });

  const toggleActive = async () => {
    if (classPack === undefined) {
      return;
    }
    setSubmissionFailure(null);
    const isActive = !classPack.isActive;
    try {
      await setClassPackActiveMutation.mutateAsync({ classPackId: classPack.id, isActive });
      showToast(translate(isActive ? "classPacks.form.activated" : "classPacks.form.deactivated"));
      router.back();
    } catch (activationError) {
      setSubmissionFailure(toSubmissionFailure(activationError));
    }
  };

  const renderField = ({ name, labelKey, placeholderKey, keyboardType }: ClassPackFieldOptions) => (
    <Controller
      control={form.control}
      name={name}
      render={({ field, fieldState }) => (
        <TextField
          label={translate(labelKey)}
          placeholder={placeholderKey ? translate(placeholderKey) : undefined}
          keyboardType={keyboardType}
          autoCapitalize={keyboardType === "url" ? "none" : undefined}
          value={field.value}
          onChangeText={field.onChange}
          onBlur={field.onBlur}
          errorMessage={fieldState.error?.message}
        />
      )}
    />
  );

  return (
    <SettingsFormScreenLayout
      title={translate(classPack ? "classPacks.form.editTitle" : "classPacks.form.newTitle")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      {renderField({
        name: "name",
        labelKey: "classPacks.form.name",
        placeholderKey: "classPacks.form.namePlaceholder",
      })}
      {renderField({
        name: "classCount",
        labelKey: "classPacks.form.classCount",
        keyboardType: "number-pad",
      })}
      {renderField({
        name: "price",
        labelKey: "classPacks.form.price",
        keyboardType: "decimal-pad",
      })}
      {renderField({
        name: "validityMonths",
        labelKey: "classPacks.form.validityMonths",
        placeholderKey: "classPacks.form.validityPlaceholder",
        keyboardType: "number-pad",
      })}
      <Controller
        control={form.control}
        name="classDurationMinutes"
        render={({ field }) => (
          <View className="gap-2">
            <AppText variant="label" tone="muted">
              {translate("classPacks.form.classDuration")}
            </AppText>
            <View className="flex-row flex-wrap gap-2">
              <Chip
                label={translate("classPacks.form.noFixedDuration")}
                isSelected={field.value.length === 0}
                onPress={() => field.onChange("")}
              />
              {commonDurationsInMinutes.map((durationMinutes) => (
                <Chip
                  key={durationMinutes}
                  label={translate("classGroups.form.durationOption", { minutes: durationMinutes })}
                  isSelected={field.value === String(durationMinutes)}
                  onPress={() => field.onChange(String(durationMinutes))}
                />
              ))}
            </View>
          </View>
        )}
      />
      {renderField({
        name: "materialUrl",
        labelKey: "classPacks.form.materialUrl",
        placeholderKey: "classPacks.form.materialUrlPlaceholder",
        keyboardType: "url",
      })}
      <View className="gap-3">
        <Button
          label={translate("common.save")}
          onPress={save}
          isLoading={saveClassPackMutation.isPending}
        />
        {classPack ? (
          <Button
            variant={classPack.isActive ? "dangerOutline" : "outline"}
            size="medium"
            label={translate(
              classPack.isActive ? "classPacks.form.deactivate" : "classPacks.form.activate",
            )}
            onPress={toggleActive}
            isLoading={setClassPackActiveMutation.isPending}
          />
        ) : null}
      </View>
    </SettingsFormScreenLayout>
  );
}

export function ClassPackFormScreen({ classPackId }: ClassPackFormScreenProps) {
  const classPacksQuery = useClassPacks(true, { enabled: classPackId !== undefined });
  if (classPackId === undefined) {
    return <ClassPackEditor />;
  }
  const classPack = classPacksQuery.data?.find((candidate) => candidate.id === classPackId);
  if (classPack === undefined) {
    return (
      <SettingsItemState
        navigation="close"
        isPending={classPacksQuery.isPending}
        isError={classPacksQuery.isError}
        notFoundMessage={translate("classPacks.form.notFound")}
        onRetry={() => classPacksQuery.refetch()}
      />
    );
  }
  return <ClassPackEditor key={classPack.id} classPack={classPack} />;
}
