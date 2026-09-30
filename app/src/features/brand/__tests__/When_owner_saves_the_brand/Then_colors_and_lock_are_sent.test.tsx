import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getBrand, updateBrand } from "@/features/brand/brandApi";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { BrandSettingsScreen } from "@/features/brand/screens/BrandSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBrand } from "@/testing/brandFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { mainBrandSwatches } from "@/theme/brandSwatches";

jest.mock("@/features/brand/brandApi");

const chosenColor = mainBrandSwatches[1];

describe("When owner saves the brand", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(buildBrand({ brandName: null }));
    jest.mocked(updateBrand).mockResolvedValue(buildBrand({ themeColor: chosenColor }));
  });

  it("Then colors and lock are sent", async () => {
    await renderWithProviders(
      <BrandProvider audience="team">
        <BrandSettingsScreen />
      </BrandProvider>,
    );

    await fireEvent.press(
      await screen.findByRole("button", {
        name: `${translate("brand.settings.mainColor")} ${chosenColor}`,
      }),
    );
    await fireEvent.press(screen.getByRole("switch", { name: translate("brand.settings.lock") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("brand.settings.save") }));

    await waitFor(() =>
      expect(updateBrand).toHaveBeenCalledWith({
        brandName: null,
        themeColor: chosenColor,
        accentColor: null,
        locksTheme: true,
      }),
    );
  });
});
