import { screen } from "@testing-library/react-native";
import { listProducts, listStockMovements } from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

const product = buildProduct();

describe("When product history has a cancelled app order", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([product]);
    jest.mocked(listStockMovements).mockResolvedValue([
      {
        id: "movement-1",
        variantId: product.variants[0].id,
        variantName: "",
        kind: "Cancellation",
        quantity: 20,
        orderId: "order-1",
        note: null,
        createdAt: "2026-10-22T11:43:20Z",
      },
    ]);
  });

  it("Then it reads reserva cancelada", async () => {
    await renderWithProviders(<ProductFormScreen productId={product.id} />);

    expect(
      await screen.findByText(`${translate("products.stock.kind.Cancellation")} · +20`),
    ).toBeOnTheScreen();
  });
});
