import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  listProducts,
  listStockMovements,
  recordStockMovement,
} from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

const product = buildProduct({
  variants: [
    { id: "variant-s-black", size: "S", color: "black", stock: 4 },
    { id: "variant-m-black", size: "M", color: "black", stock: 1 },
    { id: "variant-s-white", size: "S", color: "white", stock: 0 },
    { id: "variant-m-white", size: "M", color: "white", stock: 2 },
  ],
});

describe("When stock of a color is loaded", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([product]);
    jest.mocked(listStockMovements).mockResolvedValue([]);
    jest.mocked(recordStockMovement).mockResolvedValue(product);
  });

  it("Then the restock goes to that color and size", async () => {
    await renderWithProviders(<ProductFormScreen productId={product.id} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("products.stock.load") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("products.color.white") }));
    await fireEvent.press(screen.getByRole("button", { name: "M" }));
    await fireEvent.changeText(screen.getByLabelText(translate("products.stock.quantity")), "3");
    const loadButtons = screen.getAllByRole("button", { name: translate("products.stock.load") });
    await fireEvent.press(loadButtons[loadButtons.length - 1]);

    await waitFor(() =>
      expect(recordStockMovement).toHaveBeenCalledWith(product.id, {
        variantId: "variant-m-white",
        kind: "Restock",
        quantity: 3,
        note: null,
      }),
    );
  });
});
