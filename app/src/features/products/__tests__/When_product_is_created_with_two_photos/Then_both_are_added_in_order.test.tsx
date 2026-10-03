import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { pickCatalogImage } from "@/features/catalogImages/pickCatalogImage";
import {
  addProductImage,
  createProduct,
  reorderProductImages,
} from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildCatalogImages, buildPickedImage } from "@/testing/catalogImageFactory";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");
jest.mock("@/features/catalogImages/pickCatalogImage", () => ({
  ...jest.requireActual("@/features/catalogImages/pickCatalogImage"),
  pickCatalogImage: jest.fn(),
}));

const frontPhoto = buildPickedImage({ name: "front.png", uri: "file:///front.png" });
const backPhoto = buildPickedImage({ name: "back.png", uri: "file:///back.png" });
const createdProduct = buildProduct({ id: "new-product" });

describe("When product is created with two photos", () => {
  beforeEach(() => {
    jest
      .mocked(pickCatalogImage)
      .mockResolvedValueOnce(frontPhoto)
      .mockResolvedValueOnce(backPhoto);
    jest.mocked(createProduct).mockResolvedValue(createdProduct);
    jest
      .mocked(addProductImage)
      .mockResolvedValueOnce({ ...createdProduct, images: buildCatalogImages(1) })
      .mockResolvedValueOnce({ ...createdProduct, images: buildCatalogImages(2) });
  });

  it("Then both are added in order", async () => {
    await renderWithProviders(<ProductFormScreen />);

    await fireEvent.changeText(screen.getByLabelText(translate("products.form.name")), "Malla");
    await fireEvent.changeText(screen.getByLabelText(translate("products.form.price")), "35");
    await fireEvent.press(screen.getByRole("button", { name: translate("catalogImages.add") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("catalogImages.add") }));
    await waitFor(() => expect(screen.getAllByTestId("catalog-image")).toHaveLength(2));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() => expect(addProductImage).toHaveBeenCalledTimes(2));
    expect(addProductImage).toHaveBeenNthCalledWith(1, "new-product", frontPhoto);
    expect(addProductImage).toHaveBeenNthCalledWith(2, "new-product", backPhoto);
    expect(reorderProductImages).not.toHaveBeenCalled();
  });
});
