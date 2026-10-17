import { PropsWithChildren } from "react";
import { Platform, View, ViewStyle } from "react-native";
import { ThemeColors } from "@/theme/themeColorTokens";
import { useTheme } from "@/theme/useTheme";
import { PressableScale } from "@/ui/motion/PressableScale";

interface RingButtonProps extends PropsWithChildren {
  accessibilityLabel: string;
  onPress: () => void;
}

const ringAngle = "135deg";

function ringGradient(colors: ThemeColors): string {
  return `linear-gradient(${ringAngle}, ${colors.accent}, ${colors.primary} 50%, ${colors["primary-soft"]})`;
}

function ringStyle(colors: ThemeColors): ViewStyle {
  const gradient = ringGradient(colors);
  return Platform.OS === "web"
    ? ({ backgroundImage: gradient } as ViewStyle)
    : { experimental_backgroundImage: gradient };
}

export function RingButton({ accessibilityLabel, onPress, children }: RingButtonProps) {
  const { colors } = useTheme();
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      onPress={onPress}
      hitSlop={4}
      className="rounded-[20px] p-0.5"
      style={ringStyle(colors)}
    >
      <View className="rounded-[18px] bg-background p-0.5">{children}</View>
    </PressableScale>
  );
}
