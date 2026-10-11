import { screen } from "@testing-library/react-native";
import { getBrand } from "@/features/brand/brandApi";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { BrandSettingsScreen } from "@/features/brand/screens/BrandSettingsScreen";
import { buildBrand } from "@/testing/brandFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/brand/brandApi");

const builtInThemeNames = ["Agua", "Violeta", "Océano", "Rosa", "Ciruela", "Índigo", "Grafito"];

describe("When owner opens the brand", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(buildBrand());
  });

  it("Then every built in theme is offered by name", async () => {
    await renderWithProviders(
      <BrandProvider audience="team">
        <BrandSettingsScreen />
      </BrandProvider>,
    );
    await screen.findByRole("button", { name: "Agua" });

    const offeredThemeNames = builtInThemeNames.filter((themeName) =>
      screen.queryByRole("button", { name: themeName }),
    );

    expect(offeredThemeNames).toEqual(builtInThemeNames);
  });
});
