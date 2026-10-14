import { Image, StyleSheet } from "react-native";
import { useTheme } from "@/theme/useTheme";

const doodlePatternSource = require("../../assets/images/doodle-pattern.png");
const patternOpacity = 0.3;

export function DoodlePattern() {
  const { colors } = useTheme();
  return (
    <Image
      testID="doodle-pattern"
      source={doodlePatternSource}
      resizeMode="repeat"
      accessibilityElementsHidden
      importantForAccessibility="no-hide-descendants"
      aria-hidden
      style={[StyleSheet.absoluteFill, styles.pattern, { tintColor: colors.primary }]}
    />
  );
}

const styles = StyleSheet.create({
  pattern: {
    width: "100%",
    height: "100%",
    opacity: patternOpacity,
    pointerEvents: "none",
  },
});
