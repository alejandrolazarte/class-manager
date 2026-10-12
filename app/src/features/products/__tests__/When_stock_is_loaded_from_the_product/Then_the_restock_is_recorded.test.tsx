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

const product = buildProduct();

describe("When stock is loaded from the product", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([product]);
    jest.mocked(listStockMovements).mockResolvedValue([]);
    jest.mocked(recordStockMovement).mockResolvedValue(product);
  });

  it("Then the restock is recorded", async () => {
    await renderWithProviders(<ProductFormScreen productId={product.id} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("products.stock.load") }),
    );
    await fireEvent.changeText(screen.getByLabelText(translate("products.stock.quantity")), "5");
    const loadButtons = screen.getAllByRole("button", { name: translate("products.stock.load") });
    await fireEvent.press(loadButtons[loadButtons.length - 1]);

    await waitFor(() =>
      expect(recordStockMovement).toHaveBeenCalledWith(product.id, {
        variantId: product.variants[0].id,
        kind: "Restock",
        quantity: 5,
        note: null,
      }),
    );
    expect(await screen.findByText(translate("products.stock.saved"))).toBeOnTheScreen();
  });
});
