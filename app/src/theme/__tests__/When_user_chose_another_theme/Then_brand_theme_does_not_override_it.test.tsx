import { render, screen } from "@testing-library/react-native";
import { BrandThemeProbe } from "@/testing/BrandThemeProbe";
import { deriveBrandTheme } from "@/theme/deriveBrandTheme";
import { ThemeProvider } from "@/theme/ThemeProvider";

const brandTheme = {
  theme: deriveBrandTheme({ themeColor: "#5b3fa0", accentColor: null }),
  isLocked: false,
};

describe("When user chose another theme", () => {
  it("Then brand theme does not override it", async () => {
    await render(
      <ThemeProvider initialThemeName="ocean" initialColorSchemePreference="light">
        <BrandThemeProbe brandTheme={brandTheme} />
      </ThemeProvider>,
    );

    expect(await screen.findByText("ocean free")).toBeTruthy();
  });
});
