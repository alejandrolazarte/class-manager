import { PropsWithChildren } from "react";
import { View } from "react-native";
import { AppText } from "@/ui/AppText";

type BannerTone = "error" | "warning";

interface BannerProps extends PropsWithChildren {
  message: string;
  tone?: BannerTone;
}

const toneClassNames: Record<BannerTone, string> = {
  error: "border-danger/40 bg-danger-soft",
  warning: "border-warning/40 bg-warning-soft",
};

export function Banner({ message, tone = "error", children }: BannerProps) {
  return (
    <View
      accessibilityRole="alert"
      className={`gap-3 rounded-xl border p-3 ${toneClassNames[tone]}`}
    >
      <AppText variant="body">{message}</AppText>
      {children}
    </View>
  );
}
