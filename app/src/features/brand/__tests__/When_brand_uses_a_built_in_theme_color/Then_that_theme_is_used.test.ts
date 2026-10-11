import { toBrandTheme } from "@/features/brand/brandTheme";
import { themes } from "@/theme/themes";

describe("When brand uses a built in theme color", () => {
  it("Then that theme is used", () => {
    const brandTheme = toBrandTheme({
      themeColor: themes.violet.light.primary.toUpperCase(),
      accentColor: null,
    });

    expect(brandTheme).toBe(themes.violet);
  });
});
