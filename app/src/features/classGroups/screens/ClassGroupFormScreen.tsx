import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { View } from "react-native";
import { getNumberExtension, getStringExtension } from "@/api/problemDetails";
import { isApiError } from "@/api/httpClient";
import { ClassGroupForm } from "@/features/classGroups/components/ClassGroupForm";
import {
  classGroupErrorCodes,
  conflictingClassGroupIdExtension,
  enrollmentCountExtension,
} from "@/features/classGroups/classGroupErrorCodes";
import {
  classGroupFieldNames,
  ClassGroupFormValues,
  classGroupSchema,
  toClassGroupFormValues,
  toSaveClassGroupRequest,
} from "@/features/classGroups/classGroupSchema";
import { ClassGroup, Weekday } from "@/features/classGroups/types";
import { useClassGroupsIncludingInactive } from "@/features/classGroups/useClassGroups";
import {
  useSaveClassGroup,
  useSetClassGroupActive,
} from "@/features/classGroups/useClassGroupMutations";
import { Instructor } from "@/features/instructors/types";
import { useActiveInstructors } from "@/features/instructors/useActiveInstructors";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { LoadingScreen } from "@/ui/LoadingScreen";
import { useToast } from "@/ui/ToastProvider";

const badRequestStatus = 400;
const singleInstructorCount = 1;

interface ClassGroupFormScreenProps {
  classGroupId?: string;
  initialWeekday?: Weekday;
}

interface ClassGroupEditorProps {
  classGroup?: ClassGroup;
  initialWeekday?: Weekday;
  instructors: Instructor[];
}

type BusyConflict = { otherClassGroupId: string | undefined };

function ClassGroupEditor({ classGroup, initialWeekday, instructors }: ClassGroupEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const saveClassGroupMutation = useSaveClassGroup();
  const setClassGroupActiveMutation = useSetClassGroupActive();
  const { data: allClassGroups = [] } = useClassGroupsIncludingInactive();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [busyConflict, setBusyConflict] = useState<BusyConflict | null>(null);
  const [enrolledStudentCount, setEnrolledStudentCount] = useState(0);
  const form = useForm<ClassGroupFormValues>({
    resolver: zodResolver(classGroupSchema),
    defaultValues: toClassGroupFormValues({
      classGroup,
      initialWeekday,
      defaultInstructorId:
        instructors.length === singleInstructorCount ? instructors[0]?.id : undefined,
    }),
    mode: "onTouched",
  });
  const otherClassGroup = allClassGroups.find(
    (candidate) => candidate.id === busyConflict?.otherClassGroupId,
  );

  const handleSaveError = (saveError: unknown) => {
    if (isApiError(saveError) && saveError.hasCode(classGroupErrorCodes.hasEnrollments)) {
      setEnrolledStudentCount(getNumberExtension(saveError.problem, enrollmentCountExtension) ?? 1);
      return;
    }
    if (isApiError(saveError) && saveError.hasCode(classGroupErrorCodes.instructorBusy)) {
      setBusyConflict({
        otherClassGroupId: getStringExtension(saveError.problem, conflictingClassGroupIdExtension),
      });
      return;
    }
    const hasFieldErrors =
      isApiError(saveError) &&
      saveError.status === badRequestStatus &&
      applyServerFieldErrors(form, saveError.problem, classGroupFieldNames);
    if (!hasFieldErrors) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    setBusyConflict(null);
    try {
      await saveClassGroupMutation.mutateAsync({
        classGroupId: classGroup?.id,
        request: toSaveClassGroupRequest(formValues),
      });
      showToast(translate("classGroups.form.saved"));
      router.back();
    } catch (saveError) {
      handleSaveError(saveError);
    }
  });

  const toggleActive = async () => {
    if (classGroup === undefined) {
      return;
    }
    setSubmissionFailure(null);
    setBusyConflict(null);
    setEnrolledStudentCount(0);
    const isActive = !classGroup.isActive;
    try {
      await setClassGroupActiveMutation.mutateAsync({ classGroupId: classGroup.id, isActive });
      showToast(
        translate(isActive ? "classGroups.form.activated" : "classGroups.form.deactivated"),
      );
      router.back();
    } catch (activationError) {
      handleSaveError(activationError);
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate(classGroup ? "classGroups.form.editTitle" : "classGroups.form.newTitle")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      {busyConflict ? (
        <Banner
          tone="warning"
          message={
            otherClassGroup
              ? translate("classGroups.form.instructorBusy", {
                  instructor: otherClassGroup.instructorFullName,
                  name: otherClassGroup.name,
                  startTime: otherClassGroup.startTime,
                })
              : translate("classGroups.form.instructorBusyUnknownClass")
          }
        />
      ) : null}
      {enrolledStudentCount > 0 ? (
        <Banner
          tone="warning"
          message={translateCount("classGroups.form.hasEnrollments", enrolledStudentCount)}
        />
      ) : null}
      {instructors.length === 0 ? (
        <Banner tone="warning" message={translate("classGroups.form.noInstructors")}>
          <Button
            variant="secondary"
            label={translate("classGroups.form.addInstructor")}
            onPress={() => router.push(routes.classesNewInstructor)}
          />
        </Banner>
      ) : null}
      <ClassGroupForm form={form} instructors={instructors} />
      <View className="gap-3">
        <Button
          label={translate("common.save")}
          onPress={save}
          isLoading={saveClassGroupMutation.isPending}
        />
        {classGroup ? (
          <Button
            variant={classGroup.isActive ? "dangerOutline" : "outline"}
            size="medium"
            label={translate(
              classGroup.isActive ? "classGroups.form.deactivate" : "classGroups.form.activate",
            )}
            onPress={toggleActive}
            isLoading={setClassGroupActiveMutation.isPending}
          />
        ) : null}
      </View>
    </SettingsFormScreenLayout>
  );
}

export function ClassGroupFormScreen({ classGroupId, initialWeekday }: ClassGroupFormScreenProps) {
  const instructorsQuery = useActiveInstructors();
  const classGroupsQuery = useClassGroupsIncludingInactive();
  if (instructorsQuery.isPending) {
    return <LoadingScreen />;
  }
  const instructors = instructorsQuery.data ?? [];
  if (classGroupId === undefined) {
    return <ClassGroupEditor initialWeekday={initialWeekday} instructors={instructors} />;
  }
  const classGroup = classGroupsQuery.data?.find((candidate) => candidate.id === classGroupId);
  if (classGroup === undefined) {
    return (
      <SettingsItemState
        navigation="close"
        isPending={classGroupsQuery.isPending}
        isError={classGroupsQuery.isError}
        notFoundMessage={translate("classGroups.form.notFound")}
        onRetry={() => classGroupsQuery.refetch()}
      />
    );
  }
  const instructorsIncludingCurrent = instructors.some(
    (instructor) => instructor.id === classGroup.instructorId,
  )
    ? instructors
    : [
        ...instructors,
        { id: classGroup.instructorId, fullName: classGroup.instructorFullName, isActive: false },
      ];
  return (
    <ClassGroupEditor
      key={classGroup.id}
      classGroup={classGroup}
      instructors={instructorsIncludingCurrent}
    />
  );
}
