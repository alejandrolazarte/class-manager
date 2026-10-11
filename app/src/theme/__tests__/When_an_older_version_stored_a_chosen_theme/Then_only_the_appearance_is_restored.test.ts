import { parseThemePreferences } from "@/theme/themePreferences";

describe("When an older version stored a chosen theme", () => {
  it("Then only the appearance is restored", () => {
    const preferences = parseThemePreferences(
      JSON.stringify({ themeName: "violet", colorSchemePreference: "dark", isThemeChosen: true }),
    );

    expect(preferences).toEqual({ colorSchemePreference: "dark" });
  });
});
