import { parseThemeDefinition } from "@/theme/parseThemeDefinition";
import { themes } from "@/theme/themes";

describe("When custom theme has low contrast", () => {
  it("Then the failing pair is reported", () => {
    const result = parseThemeDefinition({
      light: { ...themes.aqua.light, "primary-foreground": "#e0f0ff" },
      dark: themes.aqua.dark,
    });

    expect(result.isValid ? [] : result.problems).toContain("light primary-foreground on primary");
  });
});
