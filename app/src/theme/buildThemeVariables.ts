import { toRgbChannels } from "@/theme/colorChannels";
import { ThemeColors, themeColorTokens } from "@/theme/themeColorTokens";

const colorVariablePrefix = "--color-";

export function buildThemeVariables(colors: ThemeColors): Record<string, string> {
  return Object.fromEntries(
    themeColorTokens.map((token) => [
      `${colorVariablePrefix}${token}`,
      toRgbChannels(colors[token]).join(" "),
    ]),
  );
}
