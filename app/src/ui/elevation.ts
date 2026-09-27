import { ViewStyle } from "react-native";
import { useTheme } from "@/theme/useTheme";

const cardShadowOpacity = 0.08;
const cardShadowRadius = 14;
const cardShadowOffset = { width: 0, height: 4 };
const cardElevation = 2;
const floatingShadowOpacity = 0.3;
const floatingShadowRadius = 18;
const floatingShadowOffset = { width: 0, height: 6 };
const floatingElevation = 6;

export type ElevationLevel = "card" | "floating";

export function useElevationStyle(level: ElevationLevel = "card"): ViewStyle {
  const { colors, colorScheme } = useTheme();
  const isFloating = level === "floating";
  if (colorScheme === "dark" && !isFloating) {
    return {};
  }
  return {
    shadowColor: isFloating ? colors.primary : colors.shadow,
    shadowOpacity: isFloating ? floatingShadowOpacity : cardShadowOpacity,
    shadowRadius: isFloating ? floatingShadowRadius : cardShadowRadius,
    shadowOffset: isFloating ? floatingShadowOffset : cardShadowOffset,
    elevation: isFloating ? floatingElevation : cardElevation,
  };
}
