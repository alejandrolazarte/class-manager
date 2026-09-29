import { vars } from "nativewind";
import { PropsWithChildren, useMemo } from "react";
import { View } from "react-native";
import { buildThemeVariables } from "@/theme/buildThemeVariables";
import { useTheme } from "@/theme/useTheme";

interface ThemeScopeProps extends PropsWithChildren {
  className?: string;
}

export function ThemeScope({ className, children }: ThemeScopeProps) {
  const { colors } = useTheme();
  const themeVariablesStyle = useMemo(() => vars(buildThemeVariables(colors)), [colors]);
  return (
    <View style={themeVariablesStyle} className={className}>
      {children}
    </View>
  );
}
