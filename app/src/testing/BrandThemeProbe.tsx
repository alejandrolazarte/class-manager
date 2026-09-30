import { useEffect } from "react";
import { Text } from "react-native";
import { BrandTheme } from "@/theme/ThemeContext";
import { useTheme } from "@/theme/useTheme";

export function BrandThemeProbe({ brandTheme }: { brandTheme: BrandTheme | null }) {
  const { themeName, isThemeLocked, setBrandTheme } = useTheme();
  useEffect(() => setBrandTheme(brandTheme), [brandTheme, setBrandTheme]);
  return <Text>{`${themeName} ${isThemeLocked ? "locked" : "free"}`}</Text>;
}
