import { contrastRatio } from "@/theme/contrastRatio";
import { ThemeColors, themeContrastPairs } from "@/theme/themeColorTokens";
import { themes } from "@/theme/themes";

const minimumTextContrastRatio = 4.5;

function findContrastFailures(colors: ThemeColors, schemeLabel: string) {
  return themeContrastPairs
    .map(({ foreground, background }) => ({
      pair: `${schemeLabel} ${foreground} on ${background}`,
      ratio: contrastRatio(colors[foreground], colors[background]),
    }))
    .filter(({ ratio }) => ratio < minimumTextContrastRatio);
}

describe("When any theme is defined", () => {
  it("Then foreground colors meet contrast minimum", () => {
    const failures = Object.entries(themes).flatMap(([themeName, theme]) => [
      ...findContrastFailures(theme.light, `${themeName} light`),
      ...findContrastFailures(theme.dark, `${themeName} dark`),
    ]);

    expect(failures).toEqual([]);
  });
});
