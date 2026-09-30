import { render, screen } from "@testing-library/react-native";
import { BrandThemeProbe } from "@/testing/BrandThemeProbe";
import { deriveBrandTheme } from "@/theme/deriveBrandTheme";
import { ThemeProvider } from "@/theme/ThemeProvider";

const brandTheme = {
  theme: deriveBrandTheme({ themeColor: "#5b3fa0", accentColor: null }),
  isLocked: false,
};

describe("When business has a brand theme", () => {
  it("Then it is the default", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <BrandThemeProbe brandTheme={brandTheme} />
      </ThemeProvider>,
    );

    expect(await screen.findByText("business free")).toBeTruthy();
  });
});
