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
  accessibilityLabel?: string;
}

const rowClassName = "min-h-[46px] flex-row items-center gap-3 px-4 py-3";

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
      {icon ? <Icon name={icon} size="medium" tone={iconTone} /> : null}
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
  return <View className="mx-4 h-px bg-border-subtle" />;
}
