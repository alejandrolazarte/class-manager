import Ionicons from "@expo/vector-icons/Ionicons";
import { ComponentProps } from "react";
import { ColorValue } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { useTheme } from "@/theme/useTheme";

type IoniconsGlyph = ComponentProps<typeof Ionicons>["name"];

const iconGlyphs = {
  today: "checkmark-circle-outline",
  classes: "calendar-outline",
  students: "people-outline",
  fees: "wallet-outline",
  settings: "settings-outline",
  add: "add",
  previous: "chevron-back",
  next: "chevron-forward",
  paid: "checkmark-circle",
} as const satisfies Record<string, IoniconsGlyph>;

const iconSizes = {
  small: 16,
  medium: 20,
  large: 24,
  extraLarge: 28,
} as const;

export type IconName = keyof typeof iconGlyphs;

export type IconSize = keyof typeof iconSizes;

interface IconProps {
  name: IconName;
  size?: IconSize;
  tone?: ThemeColorToken;
  color?: ColorValue;
  accessibilityLabel?: string;
  testID?: string;
}

export function Icon({
  name,
  size = "large",
  tone = "foreground",
  color,
  accessibilityLabel,
  testID,
}: IconProps) {
  const { colors } = useTheme();
  const isDecorative = accessibilityLabel === undefined;
  return (
    <Ionicons
      name={iconGlyphs[name]}
      size={iconSizes[size]}
      color={color ?? colors[tone]}
      testID={testID}
      accessible={!isDecorative}
      accessibilityRole={isDecorative ? undefined : "image"}
      accessibilityLabel={accessibilityLabel}
      importantForAccessibility={isDecorative ? "no-hide-descendants" : "yes"}
      aria-hidden={isDecorative}
    />
  );
}
