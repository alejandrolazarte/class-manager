import { hueDistance, toHueAndSaturation } from "@/theme/colorHue";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { colorSchemes, ThemeDefinition } from "@/theme/themes";

export const minimumStatusHueDistance = 35;
export const neutralSaturationLimit = 0.3;

const statusTokens: readonly ThemeColorToken[] = ["danger", "warning", "success"];

export interface StatusHueClash {
  pair: string;
  distance: number;
}

export function findStatusHueClashes(theme: ThemeDefinition, themeLabel: string): StatusHueClash[] {
  return colorSchemes.flatMap((colorScheme) => {
    const colors = theme[colorScheme];
    const primary = toHueAndSaturation(colors.primary);
    if (primary.saturation < neutralSaturationLimit) {
      return [];
    }
    return statusTokens
      .map((statusToken) => ({
        pair: `${themeLabel} ${colorScheme} primary near ${statusToken}`,
        distance: hueDistance(primary.hue, toHueAndSaturation(colors[statusToken]).hue),
      }))
      .filter(({ distance }) => distance < minimumStatusHueDistance);
  });
}
