import { PropsWithChildren } from "react";
import { Text, View } from "react-native";

type BannerTone = "error" | "warning";

interface BannerProps extends PropsWithChildren {
  message: string;
  tone?: BannerTone;
}

const toneClassNames: Record<BannerTone, string> = {
  error: "border-red-300 bg-red-50",
  warning: "border-amber-300 bg-amber-50",
};

export function Banner({ message, tone = "error", children }: BannerProps) {
  return (
    <View
      accessibilityRole="alert"
      className={`gap-3 rounded-xl border p-3 ${toneClassNames[tone]}`}
    >
      <Text className="text-base text-gray-900">{message}</Text>
      {children}
    </View>
  );
}
