import { screen } from "@testing-library/react-native";
import { listProducts, listStockMovements } from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

const product = buildProduct();

describe("When product history has an app order reservation", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([product]);
    jest.mocked(listStockMovements).mockResolvedValue([
      {
        id: "movement-1",
        variantId: product.variants[0].id,
        variantName: "",
        kind: "Reservation",
        quantity: -20,
        orderId: "order-1",
        note: null,
        createdAt: "2026-10-22T11:43:20Z",
      },
    ]);
  });

  it("Then it reads reserva", async () => {
    await renderWithProviders(<ProductFormScreen productId={product.id} />);

    expect(
      await screen.findByText(`${translate("products.stock.kind.Reservation")} · -20`),
    ).toBeOnTheScreen();
  });
});
