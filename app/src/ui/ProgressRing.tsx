import { PropsWithChildren } from "react";
import { View } from "react-native";
import Svg, { Circle } from "react-native-svg";
import { useTheme } from "@/theme/useTheme";

interface ProgressRingProps extends PropsWithChildren {
  ratio: number;
  accessibilityLabel: string;
}

const ringPixels = 104;
const strokePixels = 9;
const fullPercentage = 100;
const startAtTopDegrees = -90;

export function ProgressRing({ ratio, accessibilityLabel, children }: ProgressRingProps) {
  const { colors } = useTheme();
  const clampedRatio = Math.max(0, Math.min(1, ratio));
  const radius = (ringPixels - strokePixels) / 2;
  const circumference = 2 * Math.PI * radius;
  const center = ringPixels / 2;
  return (
    <View
      accessible
      accessibilityRole="progressbar"
      accessibilityLabel={accessibilityLabel}
      accessibilityValue={{
        min: 0,
        max: fullPercentage,
        now: Math.round(clampedRatio * fullPercentage),
      }}
      className="items-center justify-center"
      style={{ width: ringPixels, height: ringPixels }}
    >
      <View className="absolute inset-0">
        <Svg width={ringPixels} height={ringPixels}>
          <Circle
            cx={center}
            cy={center}
            r={radius}
            stroke={colors.muted}
            strokeWidth={strokePixels}
            fill="none"
          />
          <Circle
            cx={center}
            cy={center}
            r={radius}
            stroke={colors.primary}
            strokeWidth={strokePixels}
            strokeLinecap="round"
            strokeDasharray={`${circumference * clampedRatio} ${circumference}`}
            fill="none"
            transform={`rotate(${startAtTopDegrees} ${center} ${center})`}
          />
        </Svg>
      </View>
      <View
        className="items-center justify-center rounded-full bg-primary-soft"
        style={{ width: ringPixels - strokePixels * 3, height: ringPixels - strokePixels * 3 }}
      >
        {children}
      </View>
    </View>
  );
}
