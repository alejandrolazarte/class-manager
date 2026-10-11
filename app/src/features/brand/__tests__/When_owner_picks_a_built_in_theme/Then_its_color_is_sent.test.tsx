import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getBrand, updateBrand } from "@/features/brand/brandApi";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { BrandSettingsScreen } from "@/features/brand/screens/BrandSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBrand } from "@/testing/brandFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { themes } from "@/theme/themes";

jest.mock("@/features/brand/brandApi");

const chosenColor = themes.ocean.light.primary;

describe("When owner picks a built in theme", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(buildBrand({ brandName: null }));
    jest.mocked(updateBrand).mockResolvedValue(buildBrand({ themeColor: chosenColor }));
  });

  it("Then its color is sent", async () => {
    await renderWithProviders(
      <BrandProvider audience="team">
        <BrandSettingsScreen />
      </BrandProvider>,
    );

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("themes.ocean"),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("brand.settings.save") }));

    await waitFor(() =>
      expect(updateBrand).toHaveBeenCalledWith({
        brandName: null,
        themeColor: chosenColor,
        accentColor: null,
      }),
    );
  });
});
