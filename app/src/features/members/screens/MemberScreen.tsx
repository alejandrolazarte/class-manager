import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { useActiveInstructors } from "@/features/instructors/useActiveInstructors";
import { RoleFields } from "@/features/members/components/RoleFields";
import { freeInstructors } from "@/features/members/freeInstructors";
import { memberErrorCodes } from "@/features/members/memberErrorCodes";
import { BusinessRole, Member, Team } from "@/features/members/types";
import { useTeam } from "@/features/members/useTeam";
import { useChangeMemberRole, useRemoveMember } from "@/features/members/useTeamMutations";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { useToast } from "@/ui/ToastProvider";

interface MemberScreenProps {
  memberId: string;
}

interface MemberEditorProps {
  member: Member;
  team: Team;
}

function MemberEditor({ member, team }: MemberEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const instructorsQuery = useActiveInstructors();
  const changeRoleMutation = useChangeMemberRole(member.id);
  const removeMutation = useRemoveMember();
  const [role, setRole] = useState<BusinessRole>(member.role);
  const [instructorId, setInstructorId] = useState<string | null>(member.instructorId);
  const [instructorError, setInstructorError] = useState<string | undefined>();
  const [isConfirmingRemove, setIsConfirmingRemove] = useState(false);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const instructors = freeInstructors(instructorsQuery.data ?? [], team, member.instructorId);

  const save = async () => {
    setSubmissionFailure(null);
    if (role === "Coach" && instructorId === null) {
      setInstructorError(translate("team.invite.coachRequired"));
      return;
    }
    setInstructorError(undefined);
    try {
      await changeRoleMutation.mutateAsync({
        role,
        instructorId: role === "Coach" ? instructorId : null,
      });
      showToast(translate("team.member.saved"));
      router.back();
    } catch (saveError) {
      if (isApiError(saveError) && saveError.hasCode(memberErrorCodes.instructorTaken)) {
        setInstructorError(translate("team.invite.coachTaken"));
        return;
      }
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  const remove = async () => {
    setSubmissionFailure(null);
    try {
      await removeMutation.mutateAsync(member.id);
      showToast(translate("team.member.removed"));
      router.back();
    } catch (removeError) {
      setSubmissionFailure(toSubmissionFailure(removeError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={member.fullName}
      subtitle={member.email}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      <RoleFields
        role={role}
        instructorId={instructorId}
        instructors={instructors}
        onRoleChange={setRole}
        onInstructorChange={setInstructorId}
        instructorError={instructorError}
      />
      <Button
        label={translate("common.save")}
        onPress={save}
        isLoading={changeRoleMutation.isPending}
      />
      {isConfirmingRemove ? (
        <Banner
          tone="warning"
          icon="delete"
          message={translate("team.member.removeQuestion", { name: member.fullName })}
        >
          <View className="flex-row gap-2">
            <View className="flex-1">
              <Button
                variant="secondary"
                size="medium"
                label={translate("common.cancel")}
                onPress={() => setIsConfirmingRemove(false)}
              />
            </View>
            <View className="flex-1">
              <Button
                variant="danger"
                size="medium"
                label={translate("team.member.confirmRemove")}
                onPress={remove}
                isLoading={removeMutation.isPending}
              />
            </View>
          </View>
        </Banner>
      ) : (
        <Button
          variant="dangerOutline"
          size="medium"
          label={translate("team.member.remove")}
          onPress={() => setIsConfirmingRemove(true)}
        />
      )}
    </SettingsFormScreenLayout>
  );
}

export function MemberScreen({ memberId }: MemberScreenProps) {
  const teamQuery = useTeam();
  const member = teamQuery.data?.members.find((candidate) => candidate.id === memberId);
  if (teamQuery.data === undefined || member === undefined) {
    return (
      <SettingsItemState
        isPending={teamQuery.isPending}
        isError={teamQuery.isError}
        notFoundMessage={translate("team.member.notFound")}
        onRetry={() => teamQuery.refetch()}
      />
    );
  }
  return <MemberEditor key={member.id} member={member} team={teamQuery.data} />;
}
