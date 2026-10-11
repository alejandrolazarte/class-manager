import { screen } from "@testing-library/react-native";
import { listProducts } from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildCatalogImages } from "@/testing/catalogImageFactory";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

const maximumProductPhotos = 5;
const product = buildProduct({ images: buildCatalogImages(maximumProductPhotos) });

describe("When a product has the most photos allowed", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([product]);
  });

  it("Then no more photos can be added", async () => {
    await renderWithProviders(<ProductFormScreen productId={product.id} />);

    await screen.findByRole("button", {
      name: translate("catalogImages.remove", { position: maximumProductPhotos }),
    });
    expect(screen.queryByRole("button", { name: translate("catalogImages.add") })).toBeNull();
  });
});
