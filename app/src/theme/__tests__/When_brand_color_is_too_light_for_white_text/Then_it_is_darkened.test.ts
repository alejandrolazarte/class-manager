import { contrastRatio } from "@/theme/contrastRatio";
import { deriveBrandTheme } from "@/theme/deriveBrandTheme";

const minimumContrast = 4.5;

describe("When brand color is too light for white text", () => {
  it("Then it is darkened", () => {
    const theme = deriveBrandTheme({ themeColor: "#89d7f9", accentColor: null });

    expect(theme.light.primary).not.toBe("#89d7f9");
    expect(
      contrastRatio(theme.light["primary-foreground"], theme.light.primary),
    ).toBeGreaterThanOrEqual(minimumContrast);
  });
});
