import { ActivityIndicator } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { useTheme } from "@/theme/useTheme";

interface SpinnerProps {
  size?: "small" | "large";
  tone?: ThemeColorToken;
  className?: string;
  testID?: string;
}

export function Spinner({ size = "small", tone = "primary", className, testID }: SpinnerProps) {
  const { colors } = useTheme();
  return (
    <ActivityIndicator size={size} color={colors[tone]} className={className} testID={testID} />
  );
}
