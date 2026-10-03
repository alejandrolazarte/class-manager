import { Fragment } from "react";
import { View } from "react-native";
import { SettingsRow, SettingsRowTone } from "@/features/settings/components/SettingsRow";
import { translate, TranslationKey } from "@/i18n/translate";
import { Card } from "@/ui/Card";
import { IconName } from "@/ui/Icon";
import { ListDivider } from "@/ui/ListRow";
import { SectionTitle } from "@/ui/SectionTitle";

export interface SettingsItem {
  key: string;
  isVisible: boolean;
  icon: IconName;
  label: string;
  value?: string;
  onPress: () => void;
  isDestructive?: boolean;
}

export interface SettingsGroup {
  titleKey: TranslationKey;
  tone: SettingsRowTone;
  items: SettingsItem[];
}

const destructiveTone: SettingsRowTone = "danger";

interface SettingsGroupListProps {
  groups: SettingsGroup[];
}

export function SettingsGroupList({ groups }: SettingsGroupListProps) {
  const visibleGroups = groups
    .map((group) => ({ ...group, items: group.items.filter((item) => item.isVisible) }))
    .filter((group) => group.items.length > 0);
  return visibleGroups.map((group) => (
    <View key={group.titleKey} className="gap-1.5">
      <View className="px-1">
        <SectionTitle title={translate(group.titleKey)} isOverline />
      </View>
      <Card>
        {group.items.map((item, index) => (
          <Fragment key={item.key}>
            {index > 0 ? <ListDivider /> : null}
            <SettingsRow
              icon={item.icon}
              tone={item.isDestructive ? destructiveTone : group.tone}
              label={item.label}
              value={item.value}
              onPress={item.onPress}
              isDestructive={item.isDestructive}
            />
          </Fragment>
        ))}
      </Card>
    </View>
  ));
}
