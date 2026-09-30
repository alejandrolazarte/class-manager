import { permissions, Permission } from "@/features/members/permissions";
import { TranslationKey } from "@/i18n/translate";
import { IconName } from "@/ui/Icon";

export interface TabDefinition {
  name: string;
  titleKey: TranslationKey;
  icon: IconName;
  anyOfPermissions?: readonly Permission[];
}

export const tabDefinitions: readonly TabDefinition[] = [
  { name: "today", titleKey: "tabs.today", icon: "home" },
  { name: "classes", titleKey: "tabs.classes", icon: "classes" },
  { name: "students", titleKey: "tabs.students", icon: "students" },
  {
    name: "fees",
    titleKey: "tabs.fees",
    icon: "fees",
    anyOfPermissions: [permissions.paymentsViewAll, permissions.paymentsViewOwn],
  },
  { name: "settings", titleKey: "tabs.settings", icon: "settings" },
];

export function isTabVisible(
  tab: TabDefinition,
  memberPermissions: readonly Permission[],
): boolean {
  return (
    tab.anyOfPermissions === undefined ||
    tab.anyOfPermissions.some((permission) => memberPermissions.includes(permission))
  );
}
