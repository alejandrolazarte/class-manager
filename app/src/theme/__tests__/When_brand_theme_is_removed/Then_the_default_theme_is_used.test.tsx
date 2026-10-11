import { render, screen } from "@testing-library/react-native";
import { BrandThemeProbe } from "@/testing/BrandThemeProbe";
import { deriveBrandTheme } from "@/theme/deriveBrandTheme";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { defaultThemeName, ThemeDefinition, themes } from "@/theme/themes";

const brandTheme = deriveBrandTheme({ themeColor: "#5b3fa0", accentColor: null });

function renderWithBrandTheme(currentBrandTheme: ThemeDefinition | null) {
  return (
    <ThemeProvider initialColorSchemePreference="light">
      <BrandThemeProbe brandTheme={currentBrandTheme} />
    </ThemeProvider>
  );
}

describe("When brand theme is removed", () => {
  it("Then the default theme is used", async () => {
    const { rerender } = await render(renderWithBrandTheme(brandTheme));
    await screen.findByText(`business ${brandTheme.light.primary}`);

    await rerender(renderWithBrandTheme(null));

    expect(
      await screen.findByText(`${defaultThemeName} ${themes[defaultThemeName].light.primary}`),
    ).toBeTruthy();
  });
});
