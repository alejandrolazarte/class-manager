import { View } from "react-native";
import { Permission } from "@/features/members/permissions";
import { permissionGroups } from "@/features/roles/permissionGroups";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineSection } from "@/ui/HairlineSection";

interface PermissionSummaryProps {
  granted: readonly Permission[];
}

export function PermissionSummary({ granted }: PermissionSummaryProps) {
  return (
    <View className="gap-3">
      {permissionGroups.map((group) => {
        const lines = group.items.flatMap((item) => {
          const label = translate(item.labelKey);
          if (item.kind === "toggle") {
            return granted.includes(item.permission) ? [label] : [];
          }
          if (granted.includes(item.all)) {
            return [`${label} · ${translate("roles.scope.all")}`];
          }
          return granted.includes(item.own) ? [`${label} · ${translate("roles.scope.own")}`] : [];
        });
        if (lines.length === 0) {
          return null;
        }
        return (
          <HairlineSection key={group.titleKey} isCompact>
            <AppText variant="eyebrow" tone="muted">
              {translate(group.titleKey)}
            </AppText>
            {lines.map((line) => (
              <AppText key={line} variant="body">
                {line}
              </AppText>
            ))}
          </HairlineSection>
        );
      })}
    </View>
  );
}
