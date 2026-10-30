import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { InstructorField } from "@/features/instructors/components/InstructorField";
import { instructorErrorCodes } from "@/features/instructors/instructorErrorCodes";
import {
  instructorFieldNames,
  InstructorFormValues,
  instructorSchema,
  toSaveInstructorRequest,
} from "@/features/instructors/instructorSchema";
import { Instructor, SaveInstructorRequest } from "@/features/instructors/types";
import { useActiveInstructors } from "@/features/instructors/useActiveInstructors";
import { useSaveInstructor } from "@/features/instructors/useInstructorMutations";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { RoleChips } from "@/features/members/components/RoleChips";
import { freeInstructors } from "@/features/members/freeInstructors";
import { memberErrorCodes } from "@/features/members/memberErrorCodes";
import { permissions } from "@/features/members/permissions";
import { SystemRole } from "@/features/members/types";
import { useTeam } from "@/features/members/useTeam";
import { useInviteMember } from "@/features/members/useTeamMutations";
import { findRoleChoice, RoleChoice } from "@/features/roles/roleChoices";
import { useRoles } from "@/features/roles/useRoles";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { usePlanAllowing } from "@/features/subscriptions/PlanOfferProvider";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { useIsAtLimit } from "@/features/subscriptions/useSubscription";
import { userErrorCodes } from "@/features/users/userErrorCodes";
import { useSamePersonQuestion } from "@/features/users/useSamePersonQuestion";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { emailAddressPattern } from "@/forms/emailAddress";
import { isFilled } from "@/forms/requiredFields";
import { searchableText } from "@/forms/searchableText";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const instructorRole: SystemRole = "Instructor";
const defaultRoleKey: string = instructorRole;
const badRequestStatus = 400;
const suggestionMinimumLength = 2;
const maximumSuggestionCount = 3;

const emptyValues: InstructorFormValues = { fullName: "", email: "" };

function suggestedInstructors(instructors: Instructor[], typedName: string): Instructor[] {
  const searchedName = searchableText(typedName);
  if (searchedName.length < suggestionMinimumLength) {
    return [];
  }
  return instructors
    .filter((instructor) => searchableText(instructor.fullName).includes(searchedName))
    .slice(0, maximumSuggestionCount);
}

export function AddTeamMemberScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const canManageMembers = useCan(permissions.membersManage);
  const canManageInstructors = useCan(permissions.instructorsManage);
  const isAtTeamLimit = useIsAtLimit(featureCodes.team);
  const withinTeamPlan = usePlanAllowing(featureCodes.team, !isAtTeamLimit);
  const inviteMutation = useInviteMember();
  const saveInstructorMutation = useSaveInstructor();
  const instructorsQuery = useActiveInstructors({ enabled: canManageMembers });
  const teamQuery = useTeam({ enabled: canManageMembers });
  const { data: roles } = useRoles();
  const [roleKey, setRoleKey] = useState(defaultRoleKey);
  const [linkedInstructorId, setLinkedInstructorId] = useState<string | null>(null);
  const [instructorError, setInstructorError] = useState<string | undefined>();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const invitationQuestion = useSamePersonQuestion("people.samePerson.inStudents");
  const instructorQuestion = useSamePersonQuestion("people.samePerson.inBusiness");
  const form = useForm<InstructorFormValues>({ defaultValues: emptyValues });
  const [fullName, email] = useWatch({ control: form.control, name: ["fullName", "email"] });

  const roleChoice = findRoleChoice(roleKey, roles);
  const isInstructor = !canManageMembers || roleChoice?.needsInstructor === true;
  const instructors = freeInstructors(instructorsQuery.data ?? [], teamQuery.data);
  const isLinkingInstructor =
    canManageMembers && isInstructor && (linkedInstructorId !== null || !canManageInstructors);
  const isCreatingInstructor = isInstructor && !isLinkingInstructor;
  const isEmailRequired = !isCreatingInstructor;
  const suggestions =
    canManageMembers && isCreatingInstructor ? suggestedInstructors(instructors, fullName) : [];
  const areRequiredFieldsFilled = isCreatingInstructor
    ? isFilled(fullName)
    : isFilled(email) && (!isLinkingInstructor || linkedInstructorId !== null);
  const sendsInvitation = canManageMembers && (!isCreatingInstructor || isFilled(email));

  const linkInstructor = (instructor: Instructor) => {
    setLinkedInstructorId(instructor.id);
    setInstructorError(undefined);
    if (!isFilled(form.getValues("email")) && instructor.email !== null) {
      form.setValue("email", instructor.email);
    }
  };

  const changeLinkedInstructor = (instructorId: string | null) => {
    const instructor = instructors.find((candidate) => candidate.id === instructorId);
    if (instructor === undefined) {
      setLinkedInstructorId(null);
      return;
    }
    linkInstructor(instructor);
  };

  const invite = async (
    choice: RoleChoice,
    trimmedEmail: string,
    samePersonUserId: string | undefined,
  ) => {
    try {
      await inviteMutation.mutateAsync({
        email: trimmedEmail,
        role: choice.role,
        customRoleId: choice.customRoleId,
        instructorId: choice.needsInstructor ? linkedInstructorId : null,
        ...(samePersonUserId ? { samePersonUserId } : {}),
      });
      showToast(translate("team.invite.sent"));
      router.back();
    } catch (inviteError) {
      const showEmailError = (message: string) =>
        form.setError("email", { type: "server", message });
      if (
        invitationQuestion.askIfSamePerson(
          inviteError,
          trimmedEmail,
          (confirmedUserId) => invite(choice, trimmedEmail, confirmedUserId),
          () => showEmailError(translate("apiErrors.user.email_taken")),
        )
      ) {
        return;
      }
      if (isApiError(inviteError) && inviteError.hasCode(memberErrorCodes.alreadyMember)) {
        showEmailError(translate("team.invite.alreadyMember"));
        return;
      }
      if (isApiError(inviteError) && inviteError.hasCode(userErrorCodes.emailTaken)) {
        showEmailError(translate("apiErrors.user.email_taken"));
        return;
      }
      if (isApiError(inviteError) && inviteError.hasCode(memberErrorCodes.instructorTaken)) {
        setInstructorError(translate("team.invite.instructorTaken"));
        return;
      }
      setSubmissionFailure(toSubmissionFailure(inviteError));
    }
  };

  const inviteCreatedInstructor = async (createdInstructorId: string, instructorEmail: string) => {
    try {
      await inviteMutation.mutateAsync({
        email: instructorEmail,
        role: roleChoice?.role ?? instructorRole,
        customRoleId: roleChoice?.customRoleId ?? null,
        instructorId: createdInstructorId,
      });
      showToast(translate("instructors.form.savedAndInvited"));
    } catch {
      showToast(translate("instructors.form.savedButNotInvited"));
    }
  };

  const createInstructor = async (request: SaveInstructorRequest) => {
    try {
      const createdInstructor = await saveInstructorMutation.mutateAsync({ request });
      if (request.email !== null && canManageMembers) {
        await inviteCreatedInstructor(createdInstructor.id, request.email);
      } else {
        showToast(translate("instructors.form.saved"));
      }
      router.back();
    } catch (saveError) {
      if (
        instructorQuestion.askIfSamePerson(
          saveError,
          request.email ?? "",
          (samePersonUserId) => createInstructor({ ...request, samePersonUserId }),
          () =>
            form.setError("email", {
              type: "server",
              message: translate("apiErrors.user.email_taken"),
            }),
        )
      ) {
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
  };

  const save = async () => {
    setSubmissionFailure(null);
    form.clearErrors();
    const values = form.getValues();
    if (isCreatingInstructor) {
      const parsedValues = instructorSchema.safeParse(values);
      if (!parsedValues.success) {
        parsedValues.error.issues.forEach((issue) => {
          const fieldName = issue.path[0] as keyof InstructorFormValues;
          form.setError(fieldName, { type: "validation", message: issue.message });
        });
        return;
      }
      await createInstructor(toSaveInstructorRequest(values));
      return;
    }
    const trimmedEmail = values.email.trim();
    const isEmailValid = emailAddressPattern.test(trimmedEmail);
    const isInstructorMissing = isLinkingInstructor && linkedInstructorId === null;
    if (!isEmailValid) {
      form.setError("email", {
        type: "validation",
        message: translate("team.invite.emailInvalid"),
      });
    }
    setInstructorError(
      isInstructorMissing ? translate("team.invite.instructorRequired") : undefined,
    );
    if (!isEmailValid || isInstructorMissing || roleChoice === undefined) {
      return;
    }
    await invite(roleChoice, trimmedEmail, undefined);
  };

  const submit = sendsInvitation ? withinTeamPlan(() => void save()) : () => void save();

  return (
    <SettingsFormScreenLayout
      title={translate("team.add.title")}
      submissionFailure={submissionFailure}
      onRetry={submit}
    >
      <RequiredFieldsLegend />
      {canManageMembers ? (
        <View className="gap-2">
          <RoleChips roleKey={roleKey} onRoleChange={setRoleKey} />
          {isInstructor ? (
            <AppText variant="caption" tone="subtle">
              {translate("team.invite.instructorHint")}
            </AppText>
          ) : null}
        </View>
      ) : null}
      {isLinkingInstructor ? (
        <InstructorField
          testID="linked-instructor-picker"
          label={withRequiredMark(translate("team.invite.instructor"))}
          instructors={instructors}
          instructorId={linkedInstructorId}
          onChange={changeLinkedInstructor}
          emptyMessage={translate("team.invite.noFreeInstructors")}
          errorMessage={instructorError}
        />
      ) : null}
      {isCreatingInstructor ? (
        <View className="gap-2">
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
          {suggestions.length > 0 ? (
            <View className="gap-2">
              <AppText variant="caption" tone="subtle">
                {translate("team.add.existingInstructors")}
              </AppText>
              <View className="flex-row flex-wrap gap-2">
                {suggestions.map((instructor) => (
                  <Button
                    key={instructor.id}
                    variant="secondary"
                    size="small"
                    label={instructor.fullName}
                    accessibilityLabel={translate("team.add.useExisting", {
                      name: instructor.fullName,
                    })}
                    onPress={() => linkInstructor(instructor)}
                  />
                ))}
              </View>
            </View>
          ) : null}
        </View>
      ) : null}
      <Controller
        control={form.control}
        name="email"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("team.invite.email")}
            isRequired={isEmailRequired}
            keyboardType="email-address"
            autoCapitalize="none"
            autoComplete="email"
            hint={isCreatingInstructor ? translate("instructors.form.emailHint") : undefined}
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      {invitationQuestion.samePersonDialog}
      {instructorQuestion.samePersonDialog}
      <Button
        label={translate(isCreatingInstructor ? "common.save" : "team.invite.submit")}
        onPress={submit}
        disabled={!areRequiredFieldsFilled}
        isLoading={inviteMutation.isPending || saveInstructorMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}
