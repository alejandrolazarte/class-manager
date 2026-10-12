import { screen } from "@testing-library/react-native";
import { listProducts, listStockMovements } from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

const product = buildProduct({
  variants: [
    { id: "variant-s-black", size: "S", color: "black", stock: 4 },
    { id: "variant-s-white", size: "S", color: "white", stock: 0 },
  ],
});

describe("When product with sizes and colors is edited", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([product]);
    jest.mocked(listStockMovements).mockResolvedValue([]);
  });

  it("Then its units are shown by color and size", async () => {
    await renderWithProviders(<ProductFormScreen productId={product.id} />);

    expect(await screen.findByLabelText("S · Negro")).toHaveTextContent("4");
    expect(
      screen.getByText(translate("products.stock.soldOutItems", { items: "S · Blanco" })),
    ).toBeOnTheScreen();
  });
});
