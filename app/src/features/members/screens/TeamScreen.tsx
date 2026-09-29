import { useRouter } from "expo-router";
import { View } from "react-native";
import { useInstructorsIncludingInactive } from "@/features/instructors/useInstructorsIncludingInactive";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { roleDescription } from "@/features/members/roleLabels";
import { Invitation, Member } from "@/features/members/types";
import { useTeam } from "@/features/members/useTeam";
import { useRevokeInvitation } from "@/features/members/useTeamMutations";
import { memberRoleLabel } from "@/features/roles/roleChoices";
import { Role } from "@/features/roles/types";
import { useRoles } from "@/features/roles/useRoles";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { Icon } from "@/ui/Icon";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { Spinner } from "@/ui/Spinner";
import { StatusPill } from "@/ui/StatusPill";
import { useToast } from "@/ui/ToastProvider";

const isoDateLength = 10;

interface MemberCardProps {
  member: Member;
  roles: Role[] | undefined;
  coachFullName: string | undefined;
  onPress?: () => void;
}

function MemberCard({ member, roles, coachFullName, onPress }: MemberCardProps) {
  return (
    <Card
      onPress={onPress}
      accessibilityLabel={onPress ? member.fullName : undefined}
      className="flex-row items-center gap-3 rounded-[18px] px-3.5 py-3"
    >
      <Avatar name={member.fullName} />
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{member.fullName}</AppText>
        <AppText variant="caption" tone="subtle">
          {member.email}
        </AppText>
        <AppText variant="caption" tone="muted">
          {roleDescription(member.role, member.customRoleId, roles, coachFullName)}
        </AppText>
      </View>
      {member.isBrandOwner ? (
        <StatusPill label={translate("team.brandOwner")} tone="neutral" isSmall />
      ) : null}
      {member.isCurrentUser ? (
        <StatusPill label={translate("team.you")} tone="primary" isSmall />
      ) : null}
      {onPress ? <Icon name="next" tone="subtle-foreground" /> : null}
    </Card>
  );
}

interface InvitationCardProps {
  invitation: Invitation;
  roles: Role[] | undefined;
  canRevoke: boolean;
  isRevoking: boolean;
  onRevoke: () => void;
}

function InvitationCard({
  invitation,
  roles,
  canRevoke,
  isRevoking,
  onRevoke,
}: InvitationCardProps) {
  return (
    <Card className="flex-row items-center gap-3 rounded-[18px] px-3.5 py-3">
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{invitation.email}</AppText>
        <AppText variant="caption" tone="subtle">
          {`${memberRoleLabel(invitation.role, invitation.customRoleId, roles)} · ${translate(
            "team.expiresOn",
            {
              date: formatBirthDateForDisplay(invitation.expiresAt.slice(0, isoDateLength)),
            },
          )}`}
        </AppText>
      </View>
      {canRevoke ? (
        <Button
          variant="ghost"
          size="medium"
          label={translate("team.revoke")}
          accessibilityLabel={`${translate("team.revoke")} ${invitation.email}`}
          onPress={onRevoke}
          isLoading={isRevoking}
        />
      ) : null}
    </Card>
  );
}

export function TeamScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const canManageMembers = useCan(permissions.membersManage);
  const teamQuery = useTeam();
  const { data: roles } = useRoles();
  const instructorsQuery = useInstructorsIncludingInactive();
  const revokeMutation = useRevokeInvitation();
  const instructorNames = new Map(
    (instructorsQuery.data ?? []).map((instructor) => [instructor.id, instructor.fullName]),
  );
  const coachNameOf = (instructorId: string | null) =>
    instructorId === null ? undefined : instructorNames.get(instructorId);

  const revoke = async (invitationId: string) => {
    await revokeMutation.mutateAsync(invitationId);
    showToast(translate("team.revoked"));
  };

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("team.title")} />}
      hasFloatingAction={canManageMembers}
      overlay={
        canManageMembers ? (
          <FloatingActionButton
            label={translate("team.invite")}
            onPress={() => router.push(routes.inviteMember)}
          />
        ) : undefined
      }
    >
      {teamQuery.isPending ? <Spinner className="mt-6" /> : null}
      {teamQuery.isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => teamQuery.refetch()}
          />
        </Banner>
      ) : null}
      {teamQuery.data ? (
        <>
          <SectionTitle title={translate("team.members")} />
          {teamQuery.data.members.map((member) => (
            <MemberCard
              key={member.id}
              member={member}
              roles={roles}
              coachFullName={coachNameOf(member.instructorId)}
              onPress={
                canManageMembers && !member.isCurrentUser
                  ? () => router.push(routes.teamMember(member.id))
                  : undefined
              }
            />
          ))}
          {teamQuery.data.invitations.length > 0 ? (
            <>
              <SectionTitle title={translate("team.invitations")} />
              {teamQuery.data.invitations.map((invitation) => (
                <InvitationCard
                  key={invitation.id}
                  invitation={invitation}
                  roles={roles}
                  canRevoke={canManageMembers}
                  isRevoking={
                    revokeMutation.isPending && revokeMutation.variables === invitation.id
                  }
                  onRevoke={() => revoke(invitation.id)}
                />
              ))}
            </>
          ) : null}
        </>
      ) : null}
    </ScrollScreen>
  );
}
