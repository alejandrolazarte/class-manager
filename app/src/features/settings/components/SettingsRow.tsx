import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon, IconName } from "@/ui/Icon";

interface SettingsRowProps {
  icon: IconName;
  label: string;
  value?: string;
  onPress: () => void;
  isDestructive?: boolean;
  hasDivider: boolean;
}

export function SettingsRow({
  icon,
  label,
  value,
  onPress,
  isDestructive = false,
  hasDivider,
}: SettingsRowProps) {
  return (
    <HairlineRow
      onPress={onPress}
      accessibilityLabel={label}
      accessibilityHint={value}
      hasDivider={hasDivider}
      className="min-h-12 flex-row items-center gap-3 py-2.5"
    >
      <Icon name={icon} size="medium" tone={isDestructive ? "danger" : "muted-foreground"} />
      <View className="min-w-0 flex-1">
        <AppText variant="body" tone={isDestructive ? "danger" : "default"} numberOfLines={1}>
          {label}
        </AppText>
      </View>
      {value ? (
        <AppText variant="caption" tone="subtle" numberOfLines={1}>
          {value}
        </AppText>
      ) : null}
      {isDestructive ? null : <Icon name="next" size="medium" tone="subtle-foreground" />}
    </HairlineRow>
  );
}
