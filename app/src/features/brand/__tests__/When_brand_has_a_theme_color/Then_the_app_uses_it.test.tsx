import { screen } from "@testing-library/react-native";
import { Text } from "react-native";
import { getBrand } from "@/features/brand/brandApi";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { buildBrand } from "@/testing/brandFactory";
import { renderWithSession } from "@/testing/renderWithSession";
import { useTheme } from "@/theme/useTheme";

jest.mock("@/features/brand/brandApi");

function ThemeNameProbe() {
  const { themeName } = useTheme();
  return <Text>{`theme ${themeName}`}</Text>;
}

describe("When brand has a theme color", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(buildBrand({ themeColor: "#5b3fa0" }));
  });

  it("Then the app uses it", async () => {
    await renderWithSession(
      <BrandProvider audience="team">
        <ThemeNameProbe />
      </BrandProvider>,
    );

    expect(await screen.findByText("theme business")).toBeTruthy();
  });
});
