import { View } from "react-native";
import { SettingsRow } from "@/features/settings/components/SettingsRow";
import { translate, TranslationKey } from "@/i18n/translate";
import { IconName } from "@/ui/Icon";
import { SectionTitle } from "@/ui/SectionTitle";

export interface SettingsItem {
  key: string;
  isVisible: boolean;
  icon: IconName;
  label: string;
  value?: string;
  onPress?: () => void;
  isDestructive?: boolean;
  isExternal?: boolean;
}

export interface SettingsGroup {
  titleKey: TranslationKey;
  items: SettingsItem[];
}

interface SettingsGroupListProps {
  groups: SettingsGroup[];
}

export function SettingsGroupList({ groups }: SettingsGroupListProps) {
  const visibleGroups = groups
    .map((group) => ({ ...group, items: group.items.filter((item) => item.isVisible) }))
    .filter((group) => group.items.length > 0);
  return visibleGroups.map((group) => (
    <View key={group.titleKey} className="gap-0.5">
      <SectionTitle title={translate(group.titleKey)} isOverline />
      {group.items.map((item, index) => (
        <SettingsRow
          key={item.key}
          icon={item.icon}
          label={item.label}
          value={item.value}
          onPress={item.onPress}
          isDestructive={item.isDestructive}
          isExternal={item.isExternal}
          hasDivider={index < group.items.length - 1}
        />
      ))}
    </View>
  ));
}
