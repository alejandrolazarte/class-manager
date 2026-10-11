import { View } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText, TextTone } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { LivePulse } from "@/ui/motion/LivePulse";
import { PressableScale } from "@/ui/motion/PressableScale";

type HighlightTone = "primary" | "muted";

interface HighlightProps {
  eyebrow: string;
  title: string;
  tone?: HighlightTone;
  icon?: IconName;
  isLive?: boolean;
  actionLabel?: string;
  onPress?: () => void;
  accessibilityLabel?: string;
}

const toneClassNames: Record<HighlightTone, string> = {
  primary: "bg-primary-soft",
  muted: "bg-muted",
};

const eyebrowTones: Record<HighlightTone, TextTone> = {
  primary: "primarySoft",
  muted: "muted",
};

const iconTones: Record<HighlightTone, ThemeColorToken> = {
  primary: "primary",
  muted: "muted-foreground",
};

const highlightClassName = "min-h-[62px] flex-row items-center gap-3 rounded-[14px] px-3.5 py-3";

export function Highlight({
  eyebrow,
  title,
  tone = "primary",
  icon,
  isLive = false,
  actionLabel,
  onPress,
  accessibilityLabel,
}: HighlightProps) {
  const content = (
    <>
      {isLive ? (
        <LivePulse ringClassName="rounded-full bg-primary">
          <View className="h-2.5 w-2.5 rounded-full bg-primary" />
        </LivePulse>
      ) : null}
      {icon ? <Icon name={icon} size="medium" tone={iconTones[tone]} /> : null}
      <View className="min-w-0 flex-1">
        <AppText variant="eyebrow" tone={eyebrowTones[tone]} numberOfLines={1}>
          {eyebrow}
        </AppText>
        <AppText variant="bodyStrong" numberOfLines={1}>
          {title}
        </AppText>
      </View>
      {actionLabel ? (
        <View className="flex-row items-center gap-0.5">
          <AppText variant="bodyStrong" tone="primary">
            {actionLabel}
          </AppText>
          <Icon name="next" size="small" tone="primary" />
        </View>
      ) : onPress ? (
        <Icon name="next" size="medium" tone="subtle-foreground" />
      ) : null}
    </>
  );
  if (onPress === undefined) {
    return <View className={`${highlightClassName} ${toneClassNames[tone]}`}>{content}</View>;
  }
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      onPress={onPress}
      className={`${highlightClassName} ${toneClassNames[tone]} active:opacity-80`}
    >
      {content}
    </PressableScale>
  );
}
