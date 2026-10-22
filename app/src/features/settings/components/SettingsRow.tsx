import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon, IconName } from "@/ui/Icon";
import { StatusPill } from "@/ui/StatusPill";

interface SettingsRowProps {
  icon: IconName;
  label: string;
  value?: string;
  planBadge?: string;
  onPress?: () => void;
  isDestructive?: boolean;
  isExternal?: boolean;
  hasDivider: boolean;
}

export function SettingsRow({
  icon,
  label,
  value,
  planBadge,
  onPress,
  isDestructive = false,
  isExternal = false,
  hasDivider,
}: SettingsRowProps) {
  return (
    <HairlineRow
      onPress={onPress}
      accessibilityLabel={label}
      accessibilityHint={[planBadge, value].filter(Boolean).join(", ") || undefined}
      hasDivider={hasDivider}
      className="min-h-12 flex-row items-center gap-3 py-2.5"
    >
      <Icon name={icon} size="medium" tone={isDestructive ? "danger" : "muted-foreground"} />
      <View className="min-w-0 flex-1">
        <AppText variant="body" tone={isDestructive ? "danger" : "default"} numberOfLines={1}>
          {label}
        </AppText>
      </View>
      {planBadge ? (
        <View>
          <StatusPill label={planBadge} tone="primary" isSmall />
        </View>
      ) : null}
      {value ? (
        <AppText variant="caption" tone="subtle" numberOfLines={1}>
          {value}
        </AppText>
      ) : null}
      {isDestructive || onPress === undefined ? null : (
        <Icon name={isExternal ? "openExternal" : "next"} size="medium" tone="subtle-foreground" />
      )}
    </HairlineRow>
  );
}
