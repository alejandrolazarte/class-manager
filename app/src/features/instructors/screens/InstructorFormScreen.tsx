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
import {
  InstructorAppAccess,
  instructorAppAccess,
} from "@/features/instructors/instructorAppAccess";
import { Instructor } from "@/features/instructors/types";
import { useInstructorsIncludingInactive } from "@/features/instructors/useInstructorsIncludingInactive";
import {
  useSaveInstructor,
  useSetInstructorActive,
} from "@/features/instructors/useInstructorMutations";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { useTeam } from "@/features/members/useTeam";
import { useInviteMember } from "@/features/members/useTeamMutations";
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
  access?: InstructorAppAccess;
}

const notInvited: InstructorAppAccess = { status: "NotInvited" };

function isSameEmail(firstEmail: string, secondEmail: string): boolean {
  return firstEmail.toLocaleLowerCase() === secondEmail.toLocaleLowerCase();
}

const instructorRole = "Instructor";

function InstructorEditor({ instructor, access = notInvited }: InstructorEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const saveInstructorMutation = useSaveInstructor();
  const setInstructorActiveMutation = useSetInstructorActive();
  const inviteMemberMutation = useInviteMember();
  const canManageMembers = useCan(permissions.membersManage);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [activeClassGroupCount, setActiveClassGroupCount] = useState(0);
  const form = useForm<InstructorFormValues>({
    resolver: zodResolver(instructorSchema),
    defaultValues: toInstructorFormValues(instructor, access.member?.email),
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["fullName"]);
  const emailHints = {
    NotInvited: translate("instructors.form.emailHint"),
    Invited: translate("instructors.form.emailHintInvited"),
    Active: translate("instructors.form.emailHintActive"),
  };

  const resendInvitationIfEmailChanged = async (email: string | null) => {
    const { invitation } = access;
    if (instructor === undefined || invitation === undefined || email === null) {
      return;
    }
    if (isSameEmail(email, invitation.email)) {
      return;
    }
    try {
      await inviteMemberMutation.mutateAsync({
        email,
        role: invitation.role,
        customRoleId: invitation.customRoleId,
        instructorId: instructor.id,
      });
    } catch {
      showToast(translate("instructors.form.invitationFailed"));
    }
  };

  const inviteNewInstructor = async (createdInstructorId: string, email: string) => {
    try {
      await inviteMemberMutation.mutateAsync({
        email,
        role: instructorRole,
        customRoleId: null,
        instructorId: createdInstructorId,
      });
      showToast(translate("instructors.form.savedAndInvited"));
    } catch {
      showToast(translate("instructors.form.savedButNotInvited"));
    }
  };

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    const request = toSaveInstructorRequest(formValues);
    try {
      const savedInstructor = await saveInstructorMutation.mutateAsync({
        instructorId: instructor?.id,
        request,
      });
      if (instructor === undefined && request.email !== null && canManageMembers) {
        await inviteNewInstructor(savedInstructor.id, request.email);
      } else {
        await resendInvitationIfEmailChanged(request.email);
        showToast(translate("instructors.form.saved"));
      }
      router.back();
    } catch (saveError) {
      if (isApiError(saveError) && saveError.hasCode(instructorErrorCodes.emailUsedToSignIn)) {
        form.setError("email", {
          type: "server",
          message: translate("instructors.form.emailHintActive"),
        });
        return;
      }
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
      <Controller
        control={form.control}
        name="email"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("instructors.form.email")}
            keyboardType="email-address"
            autoCapitalize="none"
            autoComplete="email"
            hint={emailHints[access.status]}
            editable={access.status !== "Active"}
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
  const canManageMembers = useCan(permissions.membersManage);
  const instructorsQuery = useInstructorsIncludingInactive({ enabled: instructorId !== undefined });
  const teamQuery = useTeam({ enabled: instructorId !== undefined && canManageMembers });
  if (instructorId === undefined) {
    return <InstructorEditor />;
  }
  const instructor = instructorsQuery.data?.find((candidate) => candidate.id === instructorId);
  const isTeamPending = canManageMembers && teamQuery.isPending;
  if (instructor === undefined || isTeamPending) {
    return (
      <SettingsItemState
        navigation="close"
        isPending={instructorsQuery.isPending || isTeamPending}
        isError={instructorsQuery.isError}
        notFoundMessage={translate("instructors.form.notFound")}
        onRetry={() => instructorsQuery.refetch()}
      />
    );
  }
  return (
    <InstructorEditor
      key={instructor.id}
      instructor={instructor}
      access={instructorAppAccess(instructor.id, teamQuery.data)}
    />
  );
}
