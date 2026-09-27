import { useContext } from "react";
import { ThemeContext, ThemeContextValue } from "@/theme/ThemeContext";

export function useTheme(): ThemeContextValue {
  const themeContextValue = useContext(ThemeContext);
  if (!themeContextValue) {
    throw new Error("useTheme must be used inside a ThemeProvider.");
  }
  return themeContextValue;
}
