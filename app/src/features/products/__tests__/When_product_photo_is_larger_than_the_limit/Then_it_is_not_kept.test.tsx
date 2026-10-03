import { fireEvent, screen } from "@testing-library/react-native";
import {
  catalogImageMaximumSizeInBytes,
  pickCatalogImage,
} from "@/features/catalogImages/pickCatalogImage";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildPickedImage } from "@/testing/catalogImageFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");
jest.mock("@/features/catalogImages/pickCatalogImage", () => ({
  ...jest.requireActual("@/features/catalogImages/pickCatalogImage"),
  pickCatalogImage: jest.fn(),
}));

describe("When product photo is larger than the limit", () => {
  beforeEach(() => {
    jest
      .mocked(pickCatalogImage)
      .mockResolvedValue(buildPickedImage({ size: catalogImageMaximumSizeInBytes + 1 }));
  });

  it("Then it is not kept", async () => {
    await renderWithProviders(<ProductFormScreen />);

    await fireEvent.press(screen.getByRole("button", { name: translate("catalogImages.upload") }));

    expect(await screen.findByText(translate("catalogImages.tooLarge"))).toBeTruthy();
    expect(screen.queryByTestId("catalog-image")).toBeNull();
  });
});
