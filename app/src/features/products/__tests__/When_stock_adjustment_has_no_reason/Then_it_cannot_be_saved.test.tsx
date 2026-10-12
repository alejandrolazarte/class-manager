import { fireEvent, screen } from "@testing-library/react-native";
import { listProducts, listStockMovements } from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

const product = buildProduct();

describe("When stock adjustment has no reason", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([product]);
    jest.mocked(listStockMovements).mockResolvedValue([]);
  });

  it("Then it cannot be saved", async () => {
    await renderWithProviders(<ProductFormScreen productId={product.id} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("products.stock.adjust") }),
    );
    await fireEvent.changeText(screen.getByLabelText(translate("products.stock.adjustment")), "-1");

    const adjustButtons = screen.getAllByRole("button", {
      name: translate("products.stock.adjust"),
    });
    expect(adjustButtons[adjustButtons.length - 1]).toBeDisabled();
  });
});
