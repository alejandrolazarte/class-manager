import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { pickCatalogImage } from "@/features/catalogImages/pickCatalogImage";
import { createProduct, uploadProductImage } from "@/features/products/productsApi";
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

const pickedImage = buildPickedImage();
const createdProduct = buildProduct({ id: "new-product" });

describe("When product is created with a photo", () => {
  beforeEach(() => {
    jest.mocked(pickCatalogImage).mockResolvedValue(pickedImage);
    jest.mocked(createProduct).mockResolvedValue(createdProduct);
    jest.mocked(uploadProductImage).mockResolvedValue(createdProduct);
  });

  it("Then the photo is uploaded to the new product", async () => {
    await renderWithProviders(<ProductFormScreen />);

    await fireEvent.changeText(screen.getByLabelText(translate("products.form.name")), "Malla");
    await fireEvent.changeText(screen.getByLabelText(translate("products.form.price")), "35");
    await fireEvent.press(screen.getByRole("button", { name: translate("catalogImages.upload") }));
    await screen.findByTestId("catalog-image");
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(uploadProductImage).toHaveBeenCalledWith("new-product", pickedImage),
    );
  });
});
