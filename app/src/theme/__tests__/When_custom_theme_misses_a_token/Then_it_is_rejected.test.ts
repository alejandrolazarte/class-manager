import { parseThemeDefinition } from "@/theme/parseThemeDefinition";
import { themes } from "@/theme/themes";

describe("When custom theme misses a token", () => {
  it("Then it is rejected", () => {
    const { primary: _missingPrimary, ...lightWithoutPrimary } = themes.aqua.light;

    const result = parseThemeDefinition({ light: lightWithoutPrimary, dark: themes.aqua.dark });

    expect(result.isValid).toBe(false);
    expect(result.isValid ? [] : result.problems).toContain(
      "light.primary: expected a #rrggbb color",
    );
  });
});
