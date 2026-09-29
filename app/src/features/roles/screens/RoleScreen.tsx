import { useRouter } from "expo-router";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { PermissionSummary } from "@/features/roles/components/PermissionSummary";
import {
  isSystemRole,
  roleKeyOfRole,
  roleName,
  systemRoleLabel,
} from "@/features/roles/roleChoices";
import { RoleEditorScreen } from "@/features/roles/screens/RoleEditorScreen";
import { useRoles } from "@/features/roles/useRoles";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

interface RoleScreenProps {
  roleKey: string;
}

export function RoleScreen({ roleKey }: RoleScreenProps) {
  const router = useRouter();
  const canManageRoles = useCan(permissions.rolesManage);
  const rolesQuery = useRoles();
  const role = rolesQuery.data?.find((candidate) => roleKeyOfRole(candidate) === roleKey);
  if (role === undefined) {
    return (
      <SettingsItemState
        isPending={rolesQuery.isPending}
        isError={rolesQuery.isError}
        notFoundMessage={translate("roles.notFound")}
        onRetry={() => rolesQuery.refetch()}
      />
    );
  }
  if (canManageRoles && !isSystemRole(roleKey)) {
    return <RoleEditorScreen roleKey={roleKey} />;
  }
  const copiedFromLabel =
    role.copiedFrom === null
      ? null
      : isSystemRole(role.copiedFrom)
        ? systemRoleLabel(role.copiedFrom)
        : role.copiedFrom;
  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          title={roleName(role)}
          subtitle={translateCount("roles.memberCount", role.memberCount)}
        />
      }
    >
      {role.systemRole ? (
        <AppText variant="caption" tone="subtle">
          {translate("roles.systemHint")}
        </AppText>
      ) : null}
      {copiedFromLabel ? (
        <AppText variant="caption" tone="subtle">
          {translate("roles.copiedFrom", { name: copiedFromLabel })}
        </AppText>
      ) : null}
      <PermissionSummary granted={role.permissions} />
      {canManageRoles ? (
        <Button
          variant="outline"
          label={translate("roles.copy")}
          onPress={() => router.push(routes.newRole(roleKey))}
        />
      ) : null}
    </ScrollScreen>
  );
}
