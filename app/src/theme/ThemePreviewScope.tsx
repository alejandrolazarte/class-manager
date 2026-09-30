import { vars } from "nativewind";
import { PropsWithChildren, useMemo } from "react";
import { View } from "react-native";
import { buildThemeVariables } from "@/theme/buildThemeVariables";
import { ThemeContext } from "@/theme/ThemeContext";
import { ThemeColors } from "@/theme/themeColorTokens";
import { useTheme } from "@/theme/useTheme";

interface ThemePreviewScopeProps extends PropsWithChildren {
  colors: ThemeColors;
  className?: string;
}

export function ThemePreviewScope({ colors, className, children }: ThemePreviewScopeProps) {
  const parentTheme = useTheme();
  const previewTheme = useMemo(() => ({ ...parentTheme, colors }), [parentTheme, colors]);
  const themeVariablesStyle = useMemo(() => vars(buildThemeVariables(colors)), [colors]);
  return (
    <ThemeContext.Provider value={previewTheme}>
      <View style={themeVariablesStyle} className={className}>
        {children}
      </View>
    </ThemeContext.Provider>
  );
}
