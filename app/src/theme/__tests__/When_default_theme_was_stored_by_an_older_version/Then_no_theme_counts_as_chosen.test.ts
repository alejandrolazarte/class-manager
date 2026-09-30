import { parseThemePreferences } from "@/theme/themePreferences";

describe("When default theme was stored by an older version", () => {
  it("Then no theme counts as chosen", () => {
    const preferences = parseThemePreferences(
      JSON.stringify({ themeName: "aqua", colorSchemePreference: "dark" }),
    );

    expect(preferences).toEqual({ themeName: null, colorSchemePreference: "dark" });
  });
});
