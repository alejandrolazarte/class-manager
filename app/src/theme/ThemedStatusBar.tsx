import { StatusBar } from "expo-status-bar";
import { useTheme } from "@/theme/useTheme";

export function ThemedStatusBar() {
  const { colorScheme } = useTheme();
  return <StatusBar style={colorScheme === "dark" ? "light" : "dark"} />;
}
