import { PropsWithChildren } from "react";
import { View } from "react-native";
import { AppText, TextTone } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { ThemeColorToken } from "@/theme/themeColorTokens";

type BannerTone = "error" | "warning" | "info" | "success";

interface BannerProps extends PropsWithChildren {
  message: string;
  tone?: BannerTone;
  icon?: IconName;
}

const toneClassNames: Record<BannerTone, string> = {
  error: "bg-danger-soft",
  warning: "bg-warning-soft",
  info: "bg-primary-soft",
  success: "bg-success-soft",
};

const defaultIcons: Record<BannerTone, IconName> = {
  error: "error",
  warning: "warning",
  info: "notYet",
  success: "verified",
};

const iconTones: Record<BannerTone, ThemeColorToken> = {
  error: "danger",
  warning: "warning",
  info: "primary-soft-foreground",
  success: "success",
};

const messageTones: Record<BannerTone, TextTone> = {
  error: "dangerSoft",
  warning: "warningSoft",
  info: "primarySoft",
  success: "successSoft",
};

export function Banner({ message, tone = "error", icon, children }: BannerProps) {
  return (
    <View
      accessibilityRole="alert"
      className={`gap-2.5 rounded-xl px-3.5 py-3 ${toneClassNames[tone]}`}
    >
      <View className="flex-row items-start gap-2.5">
        <Icon name={icon ?? defaultIcons[tone]} size="medium" tone={iconTones[tone]} />
        <AppText variant="body" tone={messageTones[tone]} className="flex-1">
          {message}
        </AppText>
      </View>
      {children}
    </View>
  );
}
