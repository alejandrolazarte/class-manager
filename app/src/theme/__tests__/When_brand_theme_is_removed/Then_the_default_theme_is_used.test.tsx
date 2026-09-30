import { render, screen } from "@testing-library/react-native";
import { BrandThemeProbe } from "@/testing/BrandThemeProbe";
import { deriveBrandTheme } from "@/theme/deriveBrandTheme";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { defaultThemeName } from "@/theme/themes";

const brandTheme = {
  theme: deriveBrandTheme({ themeColor: "#5b3fa0", accentColor: null }),
  isLocked: false,
};

function renderWithBrandTheme(currentBrandTheme: typeof brandTheme | null) {
  return (
    <ThemeProvider initialThemeName="business" initialColorSchemePreference="light">
      <BrandThemeProbe brandTheme={currentBrandTheme} />
    </ThemeProvider>
  );
}

describe("When brand theme is removed", () => {
  it("Then the default theme is used", async () => {
    const { rerender } = await render(renderWithBrandTheme(brandTheme));
    await screen.findByText("business free");

    await rerender(renderWithBrandTheme(null));

    expect(await screen.findByText(`${defaultThemeName} free`)).toBeTruthy();
  });
});
