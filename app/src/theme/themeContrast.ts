import { contrastRatio } from "@/theme/contrastRatio";
import { ThemeColors, themeContrastPairs } from "@/theme/themeColorTokens";
import { colorSchemes, ThemeDefinition } from "@/theme/themes";

export const minimumTextContrastRatio = 4.5;

export interface ContrastFailure {
  pair: string;
  ratio: number;
}

function findSchemeContrastFailures(colors: ThemeColors, schemeLabel: string): ContrastFailure[] {
  return themeContrastPairs
    .map(({ foreground, background }) => ({
      pair: `${schemeLabel} ${foreground} on ${background}`,
      ratio: contrastRatio(colors[foreground], colors[background]),
    }))
    .filter(({ ratio }) => ratio < minimumTextContrastRatio);
}

export function findThemeContrastFailures(
  theme: ThemeDefinition,
  themeLabel?: string,
): ContrastFailure[] {
  return colorSchemes.flatMap((colorScheme) =>
    findSchemeContrastFailures(
      theme[colorScheme],
      themeLabel ? `${themeLabel} ${colorScheme}` : colorScheme,
    ),
  );
}
