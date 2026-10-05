import { useRouter } from "expo-router";
import { useState } from "react";
import { isApiError } from "@/api/httpClient";
import { useActiveInstructors } from "@/features/instructors/useActiveInstructors";
import { RoleFields } from "@/features/members/components/RoleFields";
import { freeInstructors } from "@/features/members/freeInstructors";
import { memberErrorCodes } from "@/features/members/memberErrorCodes";
import { useTeam } from "@/features/members/useTeam";
import { useInviteMember } from "@/features/members/useTeamMutations";
import { findRoleChoice } from "@/features/roles/roleChoices";
import { useRoles } from "@/features/roles/useRoles";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { emailAddressPattern } from "@/forms/emailAddress";
import { translate } from "@/i18n/translate";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";
import { isFilled } from "@/forms/requiredFields";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";

const defaultRoleKey = "Coach";

export function InviteMemberScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const inviteMutation = useInviteMember();
  const instructorsQuery = useActiveInstructors();
  const teamQuery = useTeam();
  const { data: roles } = useRoles();
  const [email, setEmail] = useState("");
  const [roleKey, setRoleKey] = useState(defaultRoleKey);
  const [instructorId, setInstructorId] = useState<string | null>(null);
  const [emailError, setEmailError] = useState<string | undefined>();
  const [instructorError, setInstructorError] = useState<string | undefined>();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const instructors = freeInstructors(instructorsQuery.data ?? [], teamQuery.data);
  const needsCoach = findRoleChoice(roleKey, roles)?.needsCoach === true;
  const areRequiredFieldsFilled = isFilled(email) && (!needsCoach || instructorId !== null);

  const invite = async () => {
    setSubmissionFailure(null);
    const trimmedEmail = email.trim();
    const isEmailValid = emailAddressPattern.test(trimmedEmail);
    const roleChoice = findRoleChoice(roleKey, roles);
    const isCoachMissing = roleChoice?.needsCoach === true && instructorId === null;
    setEmailError(isEmailValid ? undefined : translate("team.invite.emailInvalid"));
    setInstructorError(isCoachMissing ? translate("team.invite.coachRequired") : undefined);
    if (!isEmailValid || isCoachMissing || roleChoice === undefined) {
      return;
    }
    try {
      await inviteMutation.mutateAsync({
        email: trimmedEmail,
        role: roleChoice.role,
        customRoleId: roleChoice.customRoleId,
        instructorId: roleChoice.needsCoach ? instructorId : null,
      });
      showToast(translate("team.invite.sent"));
      router.back();
    } catch (inviteError) {
      if (isApiError(inviteError) && inviteError.hasCode(memberErrorCodes.alreadyMember)) {
        setEmailError(translate("team.invite.alreadyMember"));
        return;
      }
      if (isApiError(inviteError) && inviteError.hasCode(memberErrorCodes.instructorTaken)) {
        setInstructorError(translate("team.invite.coachTaken"));
        return;
      }
      setSubmissionFailure(toSubmissionFailure(inviteError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate("team.invite.title")}
      submissionFailure={submissionFailure}
      onRetry={invite}
    >
      <RequiredFieldsLegend />
      <TextField
        label={translate("team.invite.email")}
        isRequired
        keyboardType="email-address"
        autoCapitalize="none"
        autoComplete="email"
        value={email}
        onChangeText={setEmail}
        errorMessage={emailError}
      />
      <RoleFields
        roleKey={roleKey}
        instructorId={instructorId}
        instructors={instructors}
        onRoleChange={setRoleKey}
        onInstructorChange={setInstructorId}
        instructorError={instructorError}
      />
      <Button
        label={translate("team.invite.submit")}
        onPress={invite}
        disabled={!areRequiredFieldsFilled}
        isLoading={inviteMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}
