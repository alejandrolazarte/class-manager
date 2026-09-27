import { ReactNode } from "react";
import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { ThemeColorToken } from "@/theme/themeColorTokens";

interface ListRowProps {
  label: string;
  detail?: string;
  icon?: IconName;
  iconTone?: ThemeColorToken;
  onPress?: () => void;
  trailing?: ReactNode;
  isExpanded?: boolean;
  accessibilityLabel?: string;
}

export function ListRow({
  label,
  detail,
  icon,
  iconTone = "primary",
  onPress,
  trailing,
  isExpanded,
  accessibilityLabel,
}: ListRowProps) {
  const content = (
    <>
      {icon ? <Icon name={icon} tone={iconTone} /> : null}
      <View className="min-w-0 flex-1 gap-px">
        <AppText variant="bodyStrong">{label}</AppText>
        {detail ? (
          <AppText variant="caption" tone="subtle">
            {detail}
          </AppText>
        ) : null}
      </View>
      {trailing ??
        (onPress ? (
          <Icon
            name={isExpanded === undefined ? "next" : isExpanded ? "collapse" : "expand"}
            tone="subtle-foreground"
          />
        ) : null)}
    </>
  );
  if (onPress === undefined) {
    return <View className="flex-row items-center gap-3.5 px-4 py-3.5">{content}</View>;
  }
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel ?? label}
      accessibilityState={isExpanded === undefined ? undefined : { expanded: isExpanded }}
      onPress={onPress}
      className="flex-row items-center gap-3.5 px-4 py-3.5 active:bg-muted"
    >
      {content}
    </Pressable>
  );
}

export function ListDivider() {
  return <View className="mx-4 h-px bg-border" />;
}
