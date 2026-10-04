import { PropsWithChildren } from "react";
import { View } from "react-native";
import { AppText } from "@/ui/AppText";
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
  info: "bg-muted",
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
  info: "muted-foreground",
  success: "success",
};

export function Banner({ message, tone = "error", icon, children }: BannerProps) {
  return (
    <View
      accessibilityRole="alert"
      className={`gap-3 rounded-[18px] px-4 py-3.5 ${toneClassNames[tone]}`}
    >
      <View className="flex-row items-center gap-2.5">
        <Icon name={icon ?? defaultIcons[tone]} tone={iconTones[tone]} />
        <AppText variant="bodyStrong" className="flex-1 font-label">
          {message}
        </AppText>
      </View>
      {children}
    </View>
  );
}
