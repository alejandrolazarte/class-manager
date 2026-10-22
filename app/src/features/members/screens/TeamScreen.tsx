import { useRouter } from "expo-router";
import { View } from "react-native";
import { useInstructorsIncludingInactive } from "@/features/instructors/useInstructorsIncludingInactive";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { roleDescription } from "@/features/members/roleLabels";
import { Invitation, Member } from "@/features/members/types";
import { useTeam } from "@/features/members/useTeam";
import { useResendInvitation, useRevokeInvitation } from "@/features/members/useTeamMutations";
import { isPlatformRole, memberRoleLabel } from "@/features/roles/roleChoices";
import { Role } from "@/features/roles/types";
import { useRoles } from "@/features/roles/useRoles";
import { formatBirthDateForDisplay } from "@/dates/birthDateFormatting";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { StatusPill } from "@/ui/StatusPill";
import { useToast } from "@/ui/ToastProvider";
import { TitleWithPills } from "@/ui/TitleWithPills";
import { PlanUsage } from "@/features/subscriptions/components/PlanUsage";
import { usePlanAllowing } from "@/features/subscriptions/PlanOfferProvider";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { useIsAtLimit } from "@/features/subscriptions/useSubscription";

const isoDateLength = 10;

interface MemberRowProps {
  member: Member;
  roles: Role[] | undefined;
  instructorFullName: string | undefined;
  onPress?: () => void;
}

function MemberRow({ member, roles, instructorFullName, onPress }: MemberRowProps) {
  return (
    <HairlineRow onPress={onPress} accessibilityLabel={onPress ? member.fullName : undefined}>
      <Avatar name={member.fullName} size="small" />
      <View className="min-w-0 flex-1 gap-0.5">
        <TitleWithPills title={member.fullName}>
          {member.isBrandOwner ? (
            <StatusPill label={translate("team.brandOwner")} tone="neutral" isSmall />
          ) : null}
          {member.isCurrentUser ? (
            <StatusPill label={translate("team.you")} tone="primary" isSmall />
          ) : null}
        </TitleWithPills>
        <AppText variant="caption" tone="subtle">
          {member.email}
        </AppText>
        <AppText variant="caption" tone="muted">
          {roleDescription(member.role, member.customRoleId, roles, instructorFullName)}
        </AppText>
      </View>
      {onPress ? <Icon name="next" size="medium" tone="subtle-foreground" /> : null}
    </HairlineRow>
  );
}

interface InvitationRowProps {
  invitation: Invitation;
  roles: Role[] | undefined;
  canRevoke: boolean;
  isRevoking: boolean;
  isResending: boolean;
  onRevoke: () => void;
  onResend: () => void;
}

function InvitationRow({
  invitation,
  roles,
  canRevoke,
  isRevoking,
  isResending,
  onRevoke,
  onResend,
}: InvitationRowProps) {
  return (
    <HairlineRow>
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
        <View className="items-end">
          <Button
            variant="ghost"
            size="small"
            label={translate("team.resend")}
            accessibilityLabel={`${translate("team.resend")} ${invitation.email}`}
            onPress={onResend}
            isLoading={isResending}
          />
          <Button
            variant="ghost"
            size="small"
            label={translate("team.revoke")}
            accessibilityLabel={`${translate("team.revoke")} ${invitation.email}`}
            onPress={onRevoke}
            isLoading={isRevoking}
          />
        </View>
      ) : null}
    </HairlineRow>
  );
}

export function TeamScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const isAtTeamLimit = useIsAtLimit(featureCodes.team);
  const canManageMembers = useCan(permissions.membersManage);
  const withinTeamPlan = usePlanAllowing(featureCodes.team, !isAtTeamLimit);
  const teamQuery = useTeam();
  const { data: roles } = useRoles();
  const instructorsQuery = useInstructorsIncludingInactive();
  const revokeMutation = useRevokeInvitation();
  const resendMutation = useResendInvitation();
  const instructorNames = new Map(
    (instructorsQuery.data ?? []).map((instructor) => [instructor.id, instructor.fullName]),
  );
  const instructorNameOf = (instructorId: string | null) =>
    instructorId === null ? undefined : instructorNames.get(instructorId);

  const resend = async (invitationId: string) => {
    await resendMutation.mutateAsync(invitationId);
    showToast(translate("team.resent"));
  };

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
            onPress={withinTeamPlan(() => router.push(routes.inviteMember))}
          />
        ) : undefined
      }
    >
      {canManageMembers ? <PlanUsage featureCode={featureCodes.team} /> : null}
      {teamQuery.isPending ? <SkeletonList /> : null}
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
          <View>
            {teamQuery.data.members.map((member) => (
              <MemberRow
                key={member.id}
                member={member}
                roles={roles}
                instructorFullName={instructorNameOf(member.instructorId)}
                onPress={
                  canManageMembers && !member.isCurrentUser && !isPlatformRole(member.role)
                    ? () => router.push(routes.teamMember(member.id))
                    : undefined
                }
              />
            ))}
          </View>
          {teamQuery.data.invitations.length > 0 ? (
            <>
              <SectionTitle title={translate("team.invitations")} />
              <View>
                {teamQuery.data.invitations.map((invitation) => (
                  <InvitationRow
                    key={invitation.id}
                    invitation={invitation}
                    roles={roles}
                    canRevoke={canManageMembers}
                    isRevoking={
                      revokeMutation.isPending && revokeMutation.variables === invitation.id
                    }
                    isResending={
                      resendMutation.isPending && resendMutation.variables === invitation.id
                    }
                    onRevoke={() => revoke(invitation.id)}
                    onResend={() => resend(invitation.id)}
                  />
                ))}
              </View>
            </>
          ) : null}
        </>
      ) : null}
    </ScrollScreen>
  );
}
