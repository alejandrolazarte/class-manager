import { View } from "react-native";
import Svg, { Path } from "react-native-svg";
import { flameColors } from "@/theme/achievementColors";
import { useTheme } from "@/theme/useTheme";

type StreakFlameSize = "small" | "medium" | "large";

interface StreakFlameProps {
  isLit?: boolean;
  size?: StreakFlameSize;
}

const flameWidths: Record<StreakFlameSize, number> = {
  small: 22,
  medium: 48,
  large: 112,
};

const viewBoxWidth = 120;
const viewBoxHeight = 132;
const outerFlamePath =
  "M60 4c8 22 40 38 40 76a40 40 0 0 1-80 0c0-18 9-28 18-36 0 16 8 26 18 26 0-26-12-44 4-66z";
const innerFlamePath =
  "M60 58c5 12 22 20 22 40a22 22 0 0 1-44 0c0-10 5-15 10-19 1 9 5 13 10 13 0-14-5-24 2-34z";

export function StreakFlame({ isLit = true, size = "medium" }: StreakFlameProps) {
  const { colors } = useTheme();
  const width = flameWidths[size];
  const height = (width * viewBoxHeight) / viewBoxWidth;
  return (
    <View
      testID={isLit ? "streak-flame-lit" : "streak-flame-out"}
      accessibilityElementsHidden
      importantForAccessibility="no-hide-descendants"
      style={{ width, height }}
    >
      <Svg width={width} height={height} viewBox={`0 0 ${viewBoxWidth} ${viewBoxHeight}`}>
        <Path d={outerFlamePath} fill={isLit ? flameColors.outer : colors["border-strong"]} />
        {isLit ? <Path d={innerFlamePath} fill={flameColors.inner} /> : null}
      </Svg>
    </View>
  );
}
