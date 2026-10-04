import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { getStringExtension } from "@/api/problemDetails";
import { commonDurationsInMinutes } from "@/features/classGroups/classGroupSchema";
import { formatStartTimeAsTyped } from "@/features/classGroups/startTimeFormatting";
import { Instructor } from "@/features/instructors/types";
import { useActiveInstructors } from "@/features/instructors/useActiveInstructors";
import { classPackQueryKeys } from "@/features/classPacks/classPackQueryKeys";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { useCan, useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { LessonStudentPicker } from "@/features/privateLessons/components/LessonStudentPicker";
import {
  conflictingDateExtension,
  privateLessonErrorCodes,
} from "@/features/privateLessons/privateLessonErrorCodes";
import {
  privateLessonFieldNames,
  PrivateLessonFormValues,
  privateLessonLimits,
  LessonStudent,
  privateLessonSchema,
  toPrivateLessonDetails,
  toPrivateLessonFormValues,
  toSchedulePrivateLessonRequest,
} from "@/features/privateLessons/privateLessonSchema";
import { PrivateLesson } from "@/features/privateLessons/types";
import { usePrivateLesson } from "@/features/privateLessons/usePrivateLesson";
import {
  useReschedulePrivateLesson,
  useSchedulePrivateLesson,
} from "@/features/privateLessons/usePrivateLessonMutations";
import { formatLongDate, todayIsoDate } from "@/features/sessions/dates";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { formatBirthDateAsTyped } from "@/features/students/birthDateFormatting";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { LoadingScreen } from "@/ui/LoadingScreen";
import { NumberStepper } from "@/ui/NumberStepper";
import { TextField } from "@/ui/TextField";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";

const badRequestStatus = 400;
const singleInstructorCount = 1;

interface PrivateLessonFormScreenProps {
  privateLessonId?: string;
  initialDate?: string;
}

interface PrivateLessonEditorProps {
  lesson?: PrivateLesson;
  initialDate: string;
  instructors: Instructor[];
}

type CoachConflict = { date: string | undefined };

function FieldLabel({ label }: { label: string }) {
  return (
    <AppText variant="label" tone="muted">
      {label}
    </AppText>
  );
}

function FieldError({ message }: { message?: string }) {
  return message ? (
    <AppText variant="label" tone="danger" accessibilityRole="alert">
      {message}
    </AppText>
  ) : null;
}

function PrivateLessonEditor({ lesson, initialDate, instructors }: PrivateLessonEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const scheduleMutation = useSchedulePrivateLesson();
  const rescheduleMutation = useReschedulePrivateLesson(lesson?.id ?? "");
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [coachConflict, setCoachConflict] = useState<CoachConflict | null>(null);
  const isEditing = lesson !== undefined;
  const form = useForm<PrivateLessonFormValues>({
    resolver: zodResolver(privateLessonSchema),
    defaultValues: toPrivateLessonFormValues({
      lesson,
      initialDate,
      defaultInstructorId:
        instructors.length === singleInstructorCount ? instructors[0]?.id : undefined,
    }),
    mode: "onTouched",
  });
  const { control } = form;
  const queryClient = useQueryClient();
  const isTrial = useWatch({ control, name: "isTrial" });

  const applyStudentAppPackDuration = async (student: LessonStudent) => {
    if (
      isEditing ||
      form.getValues("students").length > 1 ||
      form.getFieldState("durationMinutes").isDirty
    ) {
      return;
    }
    try {
      const balance = await queryClient.fetchQuery({
        queryKey: classPackQueryKeys.balance(student.clientId),
        queryFn: () => getClassBalance(student.clientId),
      });
      const packDuration = balance.purchases.find(
        (usage) => usage.status === "Active" && usage.classDurationMinutes !== null,
      )?.classDurationMinutes;
      if (packDuration) {
        form.setValue("durationMinutes", String(packDuration));
      }
    } catch {
      return;
    }
  };

  const handleSaveError = (saveError: unknown) => {
    if (isApiError(saveError) && saveError.hasCode(privateLessonErrorCodes.instructorBusy)) {
      setCoachConflict({ date: getStringExtension(saveError.problem, conflictingDateExtension) });
      return;
    }
    const hasFieldErrors =
      isApiError(saveError) &&
      saveError.status === badRequestStatus &&
      applyServerFieldErrors(form, saveError.problem, privateLessonFieldNames);
    if (!hasFieldErrors) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    setCoachConflict(null);
    try {
      if (isEditing) {
        await rescheduleMutation.mutateAsync(toPrivateLessonDetails(formValues));
        showToast(translate("privateLessons.form.moved"));
      } else {
        const lessons = await scheduleMutation.mutateAsync(
          toSchedulePrivateLessonRequest(formValues),
        );
        showToast(translateCount("privateLessons.form.saved", lessons.length));
      }
      router.back();
    } catch (saveError) {
      handleSaveError(saveError);
    }
  });

  return (
    <SettingsFormScreenLayout
      title={translate(
        isEditing ? "privateLessons.form.editTitle" : "privateLessons.form.newTitle",
      )}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      {coachConflict ? (
        <Banner
          tone="warning"
          message={
            coachConflict.date
              ? translate("privateLessons.coachBusy", { date: formatLongDate(coachConflict.date) })
              : translate("privateLessons.coachBusyUnknownDate")
          }
        />
      ) : null}
      <Controller
        control={control}
        name="students"
        render={({ field, fieldState }) => (
          <LessonStudentPicker
            students={field.value}
            onChange={field.onChange}
            onStudentAdded={applyStudentAppPackDuration}
            errorMessage={fieldState.error?.message}
            isReadOnly={isEditing}
          />
        )}
      />
      <Controller
        control={control}
        name="instructorId"
        render={({ field, fieldState }) => (
          <View className="gap-2">
            <FieldLabel label={translate("privateLessons.form.coach")} />
            <View className="flex-row flex-wrap gap-2">
              {instructors.map((instructor) => (
                <Chip
                  key={instructor.id}
                  label={instructor.fullName}
                  isSelected={field.value === instructor.id}
                  onPress={() => field.onChange(instructor.id)}
                />
              ))}
            </View>
            <FieldError message={fieldState.error?.message} />
          </View>
        )}
      />
      <View className="flex-row gap-3">
        <View className="flex-1">
          <Controller
            control={control}
            name="date"
            render={({ field, fieldState }) => (
              <TextField
                label={translate("privateLessons.form.date")}
                placeholder={translate("privateLessons.form.datePlaceholder")}
                keyboardType="number-pad"
                value={field.value}
                onChangeText={(typedText) => field.onChange(formatBirthDateAsTyped(typedText))}
                onBlur={field.onBlur}
                errorMessage={fieldState.error?.message}
              />
            )}
          />
        </View>
        <View className="flex-1">
          <Controller
            control={control}
            name="startTime"
            render={({ field, fieldState }) => (
              <TextField
                label={translate("privateLessons.form.startTime")}
                placeholder={translate("classGroups.form.startTimePlaceholder")}
                keyboardType="number-pad"
                value={field.value}
                onChangeText={(typedText) => field.onChange(formatStartTimeAsTyped(typedText))}
                onBlur={field.onBlur}
                errorMessage={fieldState.error?.message}
              />
            )}
          />
        </View>
      </View>
      <Controller
        control={control}
        name="durationMinutes"
        render={({ field, fieldState }) => (
          <View className="gap-2">
            <FieldLabel label={translate("privateLessons.form.duration")} />
            <View className="flex-row flex-wrap gap-2">
              {commonDurationsInMinutes.map((durationMinutes) => (
                <Chip
                  key={durationMinutes}
                  label={translate("classGroups.form.durationOption", {
                    minutes: durationMinutes,
                  })}
                  isSelected={field.value === String(durationMinutes)}
                  onPress={() => field.onChange(String(durationMinutes))}
                />
              ))}
            </View>
            <TextField
              label={translate("classGroups.form.customDuration")}
              keyboardType="number-pad"
              value={field.value}
              onChangeText={field.onChange}
              onBlur={field.onBlur}
            />
            <FieldError message={fieldState.error?.message} />
          </View>
        )}
      />
      <Controller
        control={control}
        name="location"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("privateLessons.form.location")}
            placeholder={translate("privateLessons.form.locationPlaceholder")}
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={control}
        name="notes"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("privateLessons.form.notes")}
            placeholder={translate("privateLessons.form.notesPlaceholder")}
            multiline
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={control}
        name="isTrial"
        render={({ field }) => (
          <ToggleSwitch
            label={translate("privateLessons.form.isTrial")}
            value={field.value}
            onValueChange={field.onChange}
          />
        )}
      />
      {isTrial ? (
        <Controller
          control={control}
          name="trialPrice"
          render={({ field, fieldState }) => (
            <TextField
              label={translate("privateLessons.form.trialPrice")}
              keyboardType="decimal-pad"
              value={field.value}
              onChangeText={field.onChange}
              onBlur={field.onBlur}
              errorMessage={fieldState.error?.message}
            />
          )}
        />
      ) : null}
      {isEditing ? null : (
        <Controller
          control={control}
          name="repeatWeeks"
          render={({ field }) => (
            <View className="gap-1.5">
              <NumberStepper
                label={translate("privateLessons.form.repeatWeeks")}
                value={field.value}
                onChange={field.onChange}
                minimum={privateLessonLimits.minimumRepeatWeeks}
                maximum={privateLessonLimits.maximumRepeatWeeks}
                decreaseLabel={translate("privateLessons.form.fewerWeeks")}
                increaseLabel={translate("privateLessons.form.moreWeeks")}
              />
              <AppText variant="caption" tone="subtle">
                {translateCount(
                  "privateLessons.form.repeatHint",
                  Number(field.value) || privateLessonLimits.minimumRepeatWeeks,
                )}
              </AppText>
            </View>
          )}
        />
      )}
      <Button
        label={translate("common.save")}
        onPress={save}
        isLoading={scheduleMutation.isPending || rescheduleMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}

export function PrivateLessonFormScreen({
  privateLessonId,
  initialDate,
}: PrivateLessonFormScreenProps) {
  const instructorsQuery = useActiveInstructors();
  const lessonQuery = usePrivateLesson(privateLessonId);
  const canManageEveryLesson = useCan(permissions.privateLessonsManageAll);
  const { instructorId: ownInstructorId } = useCurrentMember();
  if (instructorsQuery.isPending) {
    return <LoadingScreen />;
  }
  const instructors = (instructorsQuery.data ?? []).filter(
    (instructor) => canManageEveryLesson || instructor.id === ownInstructorId,
  );
  if (privateLessonId === undefined) {
    return (
      <PrivateLessonEditor initialDate={initialDate ?? todayIsoDate()} instructors={instructors} />
    );
  }
  if (lessonQuery.data === undefined) {
    return (
      <SettingsItemState
        navigation="close"
        isPending={lessonQuery.isPending}
        isError={lessonQuery.isError}
        notFoundMessage={translate("privateLessons.notFound")}
        onRetry={() => lessonQuery.refetch()}
      />
    );
  }
  const lesson = lessonQuery.data;
  const instructorsIncludingCurrent = instructors.some(
    (instructor) => instructor.id === lesson.instructorId,
  )
    ? instructors
    : [
        ...instructors,
        { id: lesson.instructorId, fullName: lesson.instructorFullName, isActive: false },
      ];
  return (
    <PrivateLessonEditor
      key={lesson.id}
      lesson={lesson}
      initialDate={lesson.date}
      instructors={instructorsIncludingCurrent}
    />
  );
}
