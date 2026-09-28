import { useRouter } from "expo-router";
import { useState } from "react";
import { isApiError } from "@/api/httpClient";
import { useActiveInstructors } from "@/features/instructors/useActiveInstructors";
import { RoleFields } from "@/features/members/components/RoleFields";
import { freeInstructors } from "@/features/members/freeInstructors";
import { memberErrorCodes } from "@/features/members/memberErrorCodes";
import { BusinessRole } from "@/features/members/types";
import { useTeam } from "@/features/members/useTeam";
import { useInviteMember } from "@/features/members/useTeamMutations";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const defaultRole: BusinessRole = "Coach";

export function InviteMemberScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const inviteMutation = useInviteMember();
  const instructorsQuery = useActiveInstructors();
  const teamQuery = useTeam();
  const [email, setEmail] = useState("");
  const [role, setRole] = useState<BusinessRole>(defaultRole);
  const [instructorId, setInstructorId] = useState<string | null>(null);
  const [emailError, setEmailError] = useState<string | undefined>();
  const [instructorError, setInstructorError] = useState<string | undefined>();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const instructors = freeInstructors(instructorsQuery.data ?? [], teamQuery.data);

  const invite = async () => {
    setSubmissionFailure(null);
    const trimmedEmail = email.trim();
    const isEmailValid = emailPattern.test(trimmedEmail);
    const isCoachMissing = role === "Coach" && instructorId === null;
    setEmailError(isEmailValid ? undefined : translate("team.invite.emailInvalid"));
    setInstructorError(isCoachMissing ? translate("team.invite.coachRequired") : undefined);
    if (!isEmailValid || isCoachMissing) {
      return;
    }
    try {
      await inviteMutation.mutateAsync({
        email: trimmedEmail,
        role,
        instructorId: role === "Coach" ? instructorId : null,
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
      <TextField
        label={translate("team.invite.email")}
        keyboardType="email-address"
        autoCapitalize="none"
        autoComplete="email"
        value={email}
        onChangeText={setEmail}
        errorMessage={emailError}
      />
      <RoleFields
        role={role}
        instructorId={instructorId}
        instructors={instructors}
        onRoleChange={setRole}
        onInstructorChange={setInstructorId}
        instructorError={instructorError}
      />
      <Button
        label={translate("team.invite.submit")}
        onPress={invite}
        isLoading={inviteMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}
