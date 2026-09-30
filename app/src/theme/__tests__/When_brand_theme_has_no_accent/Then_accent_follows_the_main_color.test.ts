import { deriveBrandTheme } from "@/theme/deriveBrandTheme";

describe("When brand theme has no accent", () => {
  it("Then accent follows the main color", () => {
    const theme = deriveBrandTheme({ themeColor: "#5b3fa0", accentColor: null });

    expect(theme.light.accent).toBe(theme.light.primary);
    expect(theme.dark["accent-soft"]).toBe(theme.dark["primary-soft"]);
  });
});
