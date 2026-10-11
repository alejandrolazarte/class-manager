import { render, screen } from "@testing-library/react-native";
import { BrandThemeProbe } from "@/testing/BrandThemeProbe";
import { deriveBrandTheme } from "@/theme/deriveBrandTheme";
import { ThemeProvider } from "@/theme/ThemeProvider";

const brandTheme = deriveBrandTheme({ themeColor: "#5b3fa0", accentColor: null });

describe("When business has a brand theme", () => {
  it("Then it is used", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <BrandThemeProbe brandTheme={brandTheme} />
      </ThemeProvider>,
    );

    expect(await screen.findByText(`business ${brandTheme.light.primary}`)).toBeTruthy();
  });
});
