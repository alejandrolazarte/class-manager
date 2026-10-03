import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/apiErrors";
import { featureLimitReachedCode } from "@/features/catalogImages/catalogImageDraft";
import { pickCatalogImage } from "@/features/catalogImages/pickCatalogImage";
import { addProductImage, createProduct } from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildPickedImage } from "@/testing/catalogImageFactory";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");
jest.mock("@/features/catalogImages/pickCatalogImage", () => ({
  ...jest.requireActual("@/features/catalogImages/pickCatalogImage"),
  pickCatalogImage: jest.fn(),
}));

const forbiddenStatus = 403;

describe("When plan photo limit is reached", () => {
  beforeEach(() => {
    jest.mocked(pickCatalogImage).mockResolvedValue(buildPickedImage());
    jest.mocked(createProduct).mockResolvedValue(buildProduct());
    jest
      .mocked(addProductImage)
      .mockRejectedValue(new ApiError(forbiddenStatus, { code: featureLimitReachedCode }));
  });

  it("Then the product is saved with a warning", async () => {
    await renderWithProviders(<ProductFormScreen />);

    await fireEvent.changeText(screen.getByLabelText(translate("products.form.name")), "Malla");
    await fireEvent.changeText(screen.getByLabelText(translate("products.form.price")), "35");
    await fireEvent.press(screen.getByRole("button", { name: translate("catalogImages.add") }));
    await screen.findByTestId("catalog-image");
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    expect(await screen.findByText(translate("catalogImages.savedWithLimitReached"))).toBeTruthy();
  });
});
