import { useRouter } from "expo-router";
import { View } from "react-native";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { roleKeyOfRole, roleName } from "@/features/roles/roleChoices";
import { Role } from "@/features/roles/types";
import { useRoles } from "@/features/roles/useRoles";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { LockedFeatureNotice } from "@/features/subscriptions/components/LockedFeatureNotice";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { useFeature } from "@/features/subscriptions/useSubscription";

interface RoleRowProps {
  role: Role;
  onPress: () => void;
}

function RoleRow({ role, onPress }: RoleRowProps) {
  const name = roleName(role);
  return (
    <HairlineRow onPress={onPress} accessibilityLabel={name}>
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{name}</AppText>
        <AppText variant="caption" tone="muted">
          {translateCount("roles.memberCount", role.memberCount)}
        </AppText>
      </View>
      <Icon name="next" size="medium" tone="subtle-foreground" />
    </HairlineRow>
  );
}

export function RolesScreen() {
  const router = useRouter();
  const hasCustomRoles = useFeature(featureCodes.customRoles);
  const canManageRoles = useCan(permissions.rolesManage) && hasCustomRoles;
  const rolesQuery = useRoles();
  const systemRoles = (rolesQuery.data ?? []).filter((role) => role.systemRole !== null);
  const customRoles = (rolesQuery.data ?? []).filter((role) => role.systemRole === null);
  const openRole = (role: Role) => router.push(routes.role(roleKeyOfRole(role)));

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("roles.title")} />}
      hasFloatingAction={canManageRoles}
      overlay={
        canManageRoles ? (
          <FloatingActionButton
            label={translate("roles.new")}
            onPress={() => router.push(routes.newRole())}
          />
        ) : undefined
      }
    >
      {!hasCustomRoles ? (
        <LockedFeatureNotice message={translate("subscriptions.locked.customRoles")} />
      ) : null}
      {rolesQuery.isPending ? <SkeletonList /> : null}
      {rolesQuery.isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => rolesQuery.refetch()}
          />
        </Banner>
      ) : null}
      {rolesQuery.data ? (
        <>
          <SectionTitle title={translate("roles.custom")} />
          {customRoles.length === 0 ? (
            <AppText variant="body" tone="muted">
              {translate("roles.customEmpty")}
            </AppText>
          ) : null}
          <View>
            {customRoles.map((role) => (
              <RoleRow key={roleKeyOfRole(role)} role={role} onPress={() => openRole(role)} />
            ))}
          </View>
          <SectionTitle title={translate("roles.system")} />
          <AppText variant="caption" tone="subtle">
            {translate("roles.systemHint")}
          </AppText>
          <View>
            {systemRoles.map((role) => (
              <RoleRow key={roleKeyOfRole(role)} role={role} onPress={() => openRole(role)} />
            ))}
          </View>
        </>
      ) : null}
    </ScrollScreen>
  );
}
