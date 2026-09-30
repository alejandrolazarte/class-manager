import { fireEvent, screen } from "@testing-library/react-native";
import { getBrand, uploadBrandLogo } from "@/features/brand/brandApi";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { logoMaximumSizeInBytes, pickLogoFile } from "@/features/brand/pickLogoFile";
import { BrandSettingsScreen } from "@/features/brand/screens/BrandSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBrand } from "@/testing/brandFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/brand/brandApi");
jest.mock("@/features/brand/pickLogoFile", () => ({
  ...jest.requireActual("@/features/brand/pickLogoFile"),
  pickLogoFile: jest.fn(),
}));

describe("When logo is larger than the limit", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(buildBrand());
    jest.mocked(pickLogoFile).mockResolvedValue({
      name: "logo.png",
      uri: "file:///logo.png",
      mimeType: "image/png",
      size: logoMaximumSizeInBytes + 1,
    });
  });

  it("Then it is not uploaded", async () => {
    await renderWithProviders(
      <BrandProvider audience="team">
        <BrandSettingsScreen />
      </BrandProvider>,
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("brand.settings.uploadLogo") }),
    );

    expect(await screen.findByText(translate("brand.settings.logoTooLarge"))).toBeTruthy();
    expect(uploadBrandLogo).not.toHaveBeenCalled();
  });
});
