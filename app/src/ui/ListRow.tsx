import { ReactNode } from "react";
import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";
import { ThemeColorToken } from "@/theme/themeColorTokens";

interface ListRowProps {
  label: string;
  detail?: string;
  icon?: IconName;
  iconTone?: ThemeColorToken;
  onPress?: () => void;
  trailing?: ReactNode;
  isExpanded?: boolean;
  isDestructive?: boolean;
  accessibilityLabel?: string;
}

const rowClassName = "-mx-2 min-h-12 flex-row items-center gap-3 px-2 py-3";

export function ListRow({
  label,
  detail,
  icon,
  iconTone = "muted-foreground",
  onPress,
  trailing,
  isExpanded,
  isDestructive = false,
  accessibilityLabel,
}: ListRowProps) {
  const content = (
    <>
      {icon ? <Icon name={icon} size="medium" tone={iconTone} /> : null}
      <View className="min-w-0 flex-1 gap-px">
        <AppText variant="body" tone={isDestructive ? "danger" : "default"}>
          {label}
        </AppText>
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
            size="medium"
            tone="subtle-foreground"
          />
        ) : null)}
    </>
  );
  if (onPress === undefined) {
    return <View className={rowClassName}>{content}</View>;
  }
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel ?? label}
      accessibilityState={isExpanded === undefined ? undefined : { expanded: isExpanded }}
      onPress={onPress}
      className={`${rowClassName} active:bg-muted`}
    >
      {content}
    </PressableScale>
  );
}

export function ListDivider() {
  return <View className="h-px bg-border-subtle" />;
}
