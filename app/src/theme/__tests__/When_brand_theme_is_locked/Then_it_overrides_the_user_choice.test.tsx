import { render, screen } from "@testing-library/react-native";
import { BrandThemeProbe } from "@/testing/BrandThemeProbe";
import { deriveBrandTheme } from "@/theme/deriveBrandTheme";
import { ThemeProvider } from "@/theme/ThemeProvider";

const brandTheme = {
  theme: deriveBrandTheme({ themeColor: "#5b3fa0", accentColor: null }),
  isLocked: true,
};

describe("When brand theme is locked", () => {
  it("Then it overrides the user choice", async () => {
    await render(
      <ThemeProvider initialThemeName="ocean" initialColorSchemePreference="light">
        <BrandThemeProbe brandTheme={brandTheme} />
      </ThemeProvider>,
    );

    expect(await screen.findByText("business locked")).toBeTruthy();
  });
});
