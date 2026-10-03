import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listProducts, reorderProductImages, updateProduct } from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildCatalogImages } from "@/testing/catalogImageFactory";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

const images = buildCatalogImages(2);
const product = buildProduct({ images });

describe("When second product photo is made main", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([product]);
    jest.mocked(updateProduct).mockResolvedValue(product);
    jest
      .mocked(reorderProductImages)
      .mockResolvedValue({ ...product, images: [images[1], images[0]] });
  });

  it("Then the new order is sent", async () => {
    await renderWithProviders(<ProductFormScreen productId={product.id} />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("catalogImages.makeMain", { position: 2 }),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(reorderProductImages).toHaveBeenCalledWith(product.id, [images[1].id, images[0].id]),
    );
  });
});
