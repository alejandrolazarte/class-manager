import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { useActiveInstructors } from "@/features/instructors/useActiveInstructors";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { RoleFields } from "@/features/members/components/RoleFields";
import { freeInstructors } from "@/features/members/freeInstructors";
import { memberErrorCodes } from "@/features/members/memberErrorCodes";
import { permissions } from "@/features/members/permissions";
import { Member, Team } from "@/features/members/types";
import { useTeam } from "@/features/members/useTeam";
import {
  useChangeMemberRole,
  useRemoveMember,
  useSetBrandOwner,
} from "@/features/members/useTeamMutations";
import { findRoleChoice, roleKeyOf } from "@/features/roles/roleChoices";
import { useRoles } from "@/features/roles/useRoles";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { useToast } from "@/ui/ToastProvider";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";

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
  const brandOwnerMutation = useSetBrandOwner(member.id);
  const canManageBrandOwners = useCan(permissions.brandOwnersManage);
  const [isLastBrandOwner, setIsLastBrandOwner] = useState(false);
  const { data: roles } = useRoles();
  const [roleKey, setRoleKey] = useState(roleKeyOf(member.role, member.customRoleId));
  const [instructorId, setInstructorId] = useState<string | null>(member.instructorId);
  const [instructorError, setInstructorError] = useState<string | undefined>();
  const [isConfirmingRemove, setIsConfirmingRemove] = useState(false);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const instructors = freeInstructors(instructorsQuery.data ?? [], team, member.instructorId);
  const isCoachMissing =
    findRoleChoice(roleKey, roles)?.needsCoach === true && instructorId === null;

  const save = async () => {
    setSubmissionFailure(null);
    const roleChoice = findRoleChoice(roleKey, roles);
    if (roleChoice === undefined) {
      return;
    }
    if (roleChoice.needsCoach && instructorId === null) {
      setInstructorError(translate("team.invite.coachRequired"));
      return;
    }
    setInstructorError(undefined);
    try {
      await changeRoleMutation.mutateAsync({
        role: roleChoice.role,
        customRoleId: roleChoice.customRoleId,
        instructorId: roleChoice.needsCoach ? instructorId : null,
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

  const toggleBrandOwner = async () => {
    setSubmissionFailure(null);
    setIsLastBrandOwner(false);
    try {
      await brandOwnerMutation.mutateAsync(!member.isBrandOwner);
      showToast(translate("team.member.brandOwnerSaved"));
    } catch (brandOwnerError) {
      if (isApiError(brandOwnerError) && brandOwnerError.hasCode(memberErrorCodes.lastBrandOwner)) {
        setIsLastBrandOwner(true);
        return;
      }
      setSubmissionFailure(toSubmissionFailure(brandOwnerError));
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
      <RequiredFieldsLegend />
      <RoleFields
        roleKey={roleKey}
        instructorId={instructorId}
        instructors={instructors}
        onRoleChange={setRoleKey}
        onInstructorChange={setInstructorId}
        instructorError={instructorError}
      />
      <Button
        label={translate("common.save")}
        onPress={save}
        disabled={isCoachMissing}
        isLoading={changeRoleMutation.isPending}
      />
      {canManageBrandOwners ? (
        <View className="gap-2">
          <AppText variant="caption" tone="subtle">
            {translate("team.member.brandOwnerHint")}
          </AppText>
          {isLastBrandOwner ? <Banner message={translate("team.member.lastBrandOwner")} /> : null}
          <Button
            variant="outline"
            size="medium"
            label={translate(
              member.isBrandOwner ? "team.member.removeBrandOwner" : "team.member.makeBrandOwner",
            )}
            onPress={toggleBrandOwner}
            isLoading={brandOwnerMutation.isPending}
          />
        </View>
      ) : null}
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
        navigation="close"
        isPending={teamQuery.isPending}
        isError={teamQuery.isError}
        notFoundMessage={translate("team.member.notFound")}
        onRetry={() => teamQuery.refetch()}
      />
    );
  }
  return <MemberEditor key={member.id} member={member} team={teamQuery.data} />;
}
