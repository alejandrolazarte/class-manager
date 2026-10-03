import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  addProductImage,
  listProducts,
  removeProductImage,
  updateProduct,
} from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildCatalogImages } from "@/testing/catalogImageFactory";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

const images = buildCatalogImages(2);
const product = buildProduct({ images });

describe("When product photo is removed", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([product]);
    jest.mocked(updateProduct).mockResolvedValue(product);
    jest.mocked(removeProductImage).mockResolvedValue({ ...product, images: [images[1]] });
  });

  it("Then it is deleted on save", async () => {
    await renderWithProviders(<ProductFormScreen productId={product.id} />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("catalogImages.remove", { position: 1 }),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() => expect(removeProductImage).toHaveBeenCalledWith(product.id, images[0].id));
    expect(addProductImage).not.toHaveBeenCalled();
  });
});
