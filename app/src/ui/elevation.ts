import { ViewStyle } from "react-native";
import { useTheme } from "@/theme/useTheme";

const floatingShadowOpacity = 0.22;
const floatingShadowRadius = 20;
const floatingShadowOffset = { width: 0, height: 8 };
const floatingElevation = 6;

export function useFloatingShadowStyle(): ViewStyle {
  const { colors } = useTheme();
  return {
    shadowColor: colors.primary,
    shadowOpacity: floatingShadowOpacity,
    shadowRadius: floatingShadowRadius,
    shadowOffset: floatingShadowOffset,
    elevation: floatingElevation,
  };
}
