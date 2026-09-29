import { findStatusHueClashes } from "@/theme/themeStatusHues";
import { themes } from "@/theme/themes";

describe("When any theme is defined", () => {
  it("Then primary hue differs from status hues", () => {
    const clashes = Object.entries(themes).flatMap(([themeName, theme]) =>
      findStatusHueClashes(theme, themeName),
    );

    expect(clashes).toEqual([]);
  });
});
