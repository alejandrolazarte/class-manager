import { buildNavigationTheme } from "@/theme/buildNavigationTheme";
import { themes } from "@/theme/themes";

describe("When navigation theme is built", () => {
  it("Then it uses the theme colors", () => {
    const colors = themes.violet.dark;

    const navigationTheme = buildNavigationTheme(colors, "dark");

    expect(navigationTheme.dark).toBe(true);
    expect(navigationTheme.colors).toEqual({
      primary: colors.primary,
      background: colors.background,
      card: colors.surface,
      text: colors.foreground,
      border: colors.border,
      notification: colors.danger,
    });
  });
});
