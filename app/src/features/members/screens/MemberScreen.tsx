import { useRouter } from "expo-router";
import { useState } from "react";
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
import { brandOwnerRoleKey, findRoleChoice, roleKeyOf } from "@/features/roles/roleChoices";
import { useRoles } from "@/features/roles/useRoles";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
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
  const branchRoleKey = roleKeyOf(member.role, member.customRoleId);
  const isShownAsBrandOwner = member.isBrandOwner && canManageBrandOwners;
  const [roleKey, setRoleKey] = useState(isShownAsBrandOwner ? brandOwnerRoleKey : branchRoleKey);
  const [instructorId, setInstructorId] = useState<string | null>(member.instructorId);
  const [instructorError, setInstructorError] = useState<string | undefined>();
  const [isConfirmingRemove, setIsConfirmingRemove] = useState(false);
  const [isConfirmingBrandOwnerRemoval, setIsConfirmingBrandOwnerRemoval] = useState(false);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const instructors = freeInstructors(instructorsQuery.data ?? [], team, member.instructorId);
  const isInstructorLinked = member.instructorId !== null;
  const isBrandOwnerPicked = roleKey === brandOwnerRoleKey;
  const pickedRoleChoice = isBrandOwnerPicked ? undefined : findRoleChoice(roleKey, roles);
  const linkedInstructorId = (needsInstructor: boolean) =>
    member.instructorId ?? (needsInstructor ? instructorId : null);
  const isInstructorMissing =
    pickedRoleChoice?.needsInstructor === true && linkedInstructorId(true) === null;

  const changeBranchRole = async () => {
    const roleChoice = findRoleChoice(roleKey, roles);
    if (roleChoice === undefined) {
      return;
    }
    const resolvedInstructorId = linkedInstructorId(roleChoice.needsInstructor);
    if (roleChoice.needsInstructor && resolvedInstructorId === null) {
      setInstructorError(translate("team.invite.instructorRequired"));
      return;
    }
    setInstructorError(undefined);
    try {
      await changeRoleMutation.mutateAsync({
        role: roleChoice.role,
        customRoleId: roleChoice.customRoleId,
        instructorId: resolvedInstructorId,
      });
      showToast(translate("team.member.saved"));
      router.back();
    } catch (saveError) {
      if (isApiError(saveError) && saveError.hasCode(memberErrorCodes.instructorTaken)) {
        setInstructorError(translate("team.invite.instructorTaken"));
        return;
      }
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  const setBrandOwner = async (isBrandOwner: boolean) => {
    setIsLastBrandOwner(false);
    try {
      await brandOwnerMutation.mutateAsync(isBrandOwner);
      return true;
    } catch (brandOwnerError) {
      if (isApiError(brandOwnerError) && brandOwnerError.hasCode(memberErrorCodes.lastBrandOwner)) {
        setIsLastBrandOwner(true);
        return false;
      }
      setSubmissionFailure(toSubmissionFailure(brandOwnerError));
      return false;
    }
  };

  const removeBrandOwnerAndSave = async () => {
    setIsConfirmingBrandOwnerRemoval(false);
    if (!(await setBrandOwner(false))) {
      return;
    }
    if (roleKey === branchRoleKey) {
      showToast(translate("team.member.brandOwnerSaved"));
      router.back();
      return;
    }
    await changeBranchRole();
  };

  const save = async () => {
    setSubmissionFailure(null);
    if (isBrandOwnerPicked) {
      if (!member.isBrandOwner && !(await setBrandOwner(true))) {
        return;
      }
      showToast(translate("team.member.brandOwnerSaved"));
      router.back();
      return;
    }
    if (isShownAsBrandOwner) {
      setIsConfirmingBrandOwnerRemoval(true);
      return;
    }
    await changeBranchRole();
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
      title={
        isInstructorLinked
          ? translate("team.member.roleTitle", { name: member.fullName })
          : member.fullName
      }
      subtitle={isInstructorLinked ? translate("team.member.linkedInstructorHint") : member.email}
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
        includesBrandOwner={canManageBrandOwners}
        isInstructorLinked={isInstructorLinked}
      />
      {isLastBrandOwner ? <Banner message={translate("team.member.lastBrandOwner")} /> : null}
      {isConfirmingBrandOwnerRemoval ? (
        <ConfirmDialog
          title={translate("team.member.removeBrandOwnerQuestion", { name: member.fullName })}
          message={translate("team.member.removeBrandOwnerMessage")}
          confirmLabel={translate("team.member.confirmRemoveBrandOwner")}
          onCancel={() => setIsConfirmingBrandOwnerRemoval(false)}
          onConfirm={removeBrandOwnerAndSave}
          isConfirming={brandOwnerMutation.isPending}
        />
      ) : null}
      <Button
        label={translate("common.save")}
        onPress={save}
        disabled={isInstructorMissing}
        isLoading={changeRoleMutation.isPending || brandOwnerMutation.isPending}
      />
      {isConfirmingRemove ? (
        <ConfirmDialog
          title={translate("team.member.removeQuestion", { name: member.fullName })}
          message={translate("team.member.removeMessage")}
          confirmLabel={translate("team.member.confirmRemove")}
          onCancel={() => setIsConfirmingRemove(false)}
          onConfirm={remove}
          isConfirming={removeMutation.isPending}
        />
      ) : null}
      <Button
        variant="dangerOutline"
        size="medium"
        label={translate("team.member.remove")}
        onPress={() => setIsConfirmingRemove(true)}
      />
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
