import { findThemeContrastFailures } from "@/theme/themeContrast";
import { themes } from "@/theme/themes";

describe("When any theme is defined", () => {
  it("Then foreground colors meet contrast minimum", () => {
    const failures = Object.entries(themes).flatMap(([themeName, theme]) =>
      findThemeContrastFailures(theme, themeName),
    );

    expect(failures).toEqual([]);
  });
});
