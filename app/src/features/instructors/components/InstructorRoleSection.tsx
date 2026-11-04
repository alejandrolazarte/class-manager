import { useRouter } from "expo-router";
import { View } from "react-native";
import { instructorAppAccess } from "@/features/instructors/instructorAppAccess";
import { useTeam } from "@/features/members/useTeam";
import { isPlatformRole, memberRoleLabel } from "@/features/roles/roleChoices";
import { useRoles } from "@/features/roles/useRoles";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Icon } from "@/ui/Icon";

interface InstructorRoleSectionProps {
  instructorId: string;
}

export function InstructorRoleSection({ instructorId }: InstructorRoleSectionProps) {
  const router = useRouter();
  const teamQuery = useTeam();
  const { data: roles } = useRoles();
  const { member } = instructorAppAccess(instructorId, teamQuery.data);
  if (member === undefined || member.isCurrentUser || isPlatformRole(member.role)) {
    return null;
  }
  return (
    <View className="flex-row items-center gap-2.5 rounded-2xl border border-border px-4 py-3">
      <Icon name="roles" size="medium" tone="muted-foreground" />
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{translate("team.invite.role")}</AppText>
        <AppText variant="footnote" tone="muted">
          {member.isBrandOwner
            ? translate("roles.BrandOwner")
            : memberRoleLabel(member.role, member.customRoleId, roles)}
        </AppText>
      </View>
      <Button
        variant="secondary"
        size="small"
        label={translate("team.member.changeRole")}
        accessibilityLabel={translate("team.member.changeRoleAccessibility", {
          name: member.fullName,
        })}
        onPress={() => router.push(routes.teamMember(member.id))}
      />
    </View>
  );
}
