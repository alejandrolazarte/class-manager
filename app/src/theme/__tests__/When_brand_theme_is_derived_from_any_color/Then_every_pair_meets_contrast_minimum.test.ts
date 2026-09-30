import { deriveBrandTheme } from "@/theme/deriveBrandTheme";
import { findThemeContrastFailures } from "@/theme/themeContrast";

const brandColors = [
  "#0076b4",
  "#5b3fa0",
  "#1d2a33",
  "#ffffff",
  "#000000",
  "#f4d35e",
  "#d7f25a",
  "#89d7f9",
  "#c2410c",
  "#808080",
];

describe("When brand theme is derived from any color", () => {
  it.each(brandColors)("Then every pair meets contrast minimum for %s", (brandColor) => {
    const theme = deriveBrandTheme({ themeColor: brandColor, accentColor: brandColor });

    expect(findThemeContrastFailures(theme)).toEqual([]);
  });
});
