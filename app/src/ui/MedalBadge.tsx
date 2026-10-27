import { useId } from "react";
import { View } from "react-native";
import Svg, {
  Circle,
  Defs,
  Ellipse,
  LinearGradient,
  Path,
  Polygon,
  RadialGradient,
  Rect,
  Stop,
} from "react-native-svg";
import { MedalMetal, medalMetalColors, medalShineColor } from "@/theme/achievementColors";
import { useTheme } from "@/theme/useTheme";
import { Icon, IconName, IconSize } from "@/ui/Icon";

type MedalBadgeSize = "small" | "medium" | "hero";

interface MedalBadgeProps {
  icon: IconName;
  metal: MedalMetal | null;
  size?: MedalBadgeSize;
}

const viewBoxSize = 100;
const discCenterRatio = 0.66;

const badgeSizes: Record<MedalBadgeSize, number> = {
  small: 44,
  medium: 76,
  hero: 200,
};

const iconSizesByBadge: Record<MedalBadgeSize, IconSize> = {
  small: "small",
  medium: "large",
  hero: "hero",
};

const iconPixels: Record<MedalBadgeSize, number> = {
  small: 16,
  medium: 22,
  hero: 60,
};

function gradientId(prefix: string, reactId: string): string {
  return `${prefix}${reactId.replace(/[^a-zA-Z0-9]/g, "")}`;
}

export function MedalBadge({ icon, metal, size = "medium" }: MedalBadgeProps) {
  const { colors } = useTheme();
  const reactId = useId();
  const rimId = gradientId("rim", reactId);
  const faceId = gradientId("face", reactId);
  const isLocked = metal === null;
  const metalColors = isLocked
    ? {
        rimLight: colors.muted,
        rimDark: colors["border-strong"],
        faceLight: colors.surface,
        faceDark: colors.muted,
        ink: colors["disabled-foreground"],
      }
    : medalMetalColors[metal];
  const ribbonFront = isLocked ? colors.border : colors.primary;
  const ribbonBack = isLocked ? colors["border-strong"] : colors["primary-strong"];
  const pixels = badgeSizes[size];
  const iconTop = pixels * discCenterRatio - iconPixels[size] / 2;
  return (
    <View
      testID="medal-badge"
      accessibilityElementsHidden
      importantForAccessibility="no-hide-descendants"
      style={{ width: pixels, height: pixels }}
    >
      <Svg width={pixels} height={pixels} viewBox={`0 0 ${viewBoxSize} ${viewBoxSize}`}>
        <Defs>
          <LinearGradient id={rimId} x1="0" y1="0" x2="1" y2="1">
            <Stop offset="0" stopColor={metalColors.rimLight} />
            <Stop offset="1" stopColor={metalColors.rimDark} />
          </LinearGradient>
          <RadialGradient id={faceId} cx="0.35" cy="0.3" r="0.8">
            <Stop offset="0" stopColor={metalColors.faceLight} />
            <Stop offset="1" stopColor={metalColors.faceDark} />
          </RadialGradient>
        </Defs>
        <Ellipse cx="50" cy="97" rx="22" ry="2.5" fill={colors.foreground} opacity={0.1} />
        <Polygon points="28,2 44,2 58,42 42,42" fill={ribbonBack} />
        <Polygon points="72,2 56,2 42,42 58,42" fill={ribbonFront} />
        <Rect x="43" y="36" width="14" height="8" rx="3" fill={metalColors.rimDark} />
        <Circle cx="50" cy="66" r="30" fill={`url(#${rimId})`} />
        <Circle
          cx="50"
          cy="66"
          r="23"
          fill={`url(#${faceId})`}
          stroke={metalColors.rimDark}
          strokeOpacity={0.45}
          strokeWidth={1.5}
        />
        {isLocked ? null : (
          <Path
            d="M30 56 A22 22 0 0 1 48 42"
            fill="none"
            stroke={medalShineColor}
            strokeOpacity={0.6}
            strokeWidth={3}
            strokeLinecap="round"
          />
        )}
      </Svg>
      <View className="absolute inset-x-0 items-center" style={{ top: iconTop }}>
        <Icon
          name={isLocked ? "locked" : icon}
          size={iconSizesByBadge[size]}
          color={metalColors.ink}
        />
      </View>
    </View>
  );
}
