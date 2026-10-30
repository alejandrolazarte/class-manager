import { View } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText, TextTone } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";

export type StatusTone = "success" | "warning" | "danger" | "neutral" | "primary";

interface StatusPillProps {
  label: string;
  tone: StatusTone;
  icon?: IconName;
  isSmall?: boolean;
}

const toneClassNames: Record<StatusTone, string> = {
  success: "bg-success-soft",
  warning: "bg-warning-soft",
  danger: "bg-danger-soft",
  neutral: "bg-muted",
  primary: "bg-primary-soft",
};

const textTones: Record<StatusTone, TextTone> = {
  success: "successSoft",
  warning: "warningSoft",
  danger: "dangerSoft",
  neutral: "muted",
  primary: "primarySoft",
};

const iconTones: Record<StatusTone, ThemeColorToken> = {
  success: "success-soft-foreground",
  warning: "warning-soft-foreground",
  danger: "danger-soft-foreground",
  neutral: "muted-foreground",
  primary: "primary-soft-foreground",
};

export function StatusPill({ label, tone, icon, isSmall = false }: StatusPillProps) {
  return (
    <View
      className={`flex-row items-center gap-1 self-start rounded-full ${isSmall ? "px-[7px] py-0.5" : "px-2 py-[3px]"} ${toneClassNames[tone]}`}
    >
      {icon === undefined ? null : <Icon name={icon} size="tiny" tone={iconTones[tone]} />}
      <AppText variant="pill" tone={textTones[tone]} numberOfLines={1}>
        {label}
      </AppText>
    </View>
  );
}
