import { useEffect } from "react";
import { Text } from "react-native";
import { ThemeDefinition } from "@/theme/themes";
import { useTheme } from "@/theme/useTheme";

export function BrandThemeProbe({ brandTheme }: { brandTheme: ThemeDefinition | null }) {
  const { themeName, colors, setBrandTheme } = useTheme();
  useEffect(() => setBrandTheme(brandTheme), [brandTheme, setBrandTheme]);
  return <Text>{`${themeName} ${colors.primary}`}</Text>;
}
