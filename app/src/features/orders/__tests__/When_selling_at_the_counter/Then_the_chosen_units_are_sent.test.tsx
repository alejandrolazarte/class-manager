import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createCounterSale } from "@/features/orders/ordersApi";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { translate } from "@/i18n/translate";
import { buildOrder, buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/products/productsApi");

describe("When selling at the counter", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([buildProduct()]);
    jest.mocked(createCounterSale).mockResolvedValue(buildOrder());
  });

  it("Then the chosen units are sent", async () => {
    const product = buildProduct();
    await renderWithProviders(<CounterSaleScreen />);

    const increase = await screen.findByLabelText(
      translate("orders.counterSale.increase", { name: product.name }),
    );
    await fireEvent.press(increase);
    await fireEvent.press(increase);
    await fireEvent.press(screen.getByRole("button", { name: /Cobrar/ }));

    await waitFor(() =>
      expect(createCounterSale).toHaveBeenCalledWith({
        clientId: null,
        lines: [
          {
            classPackId: null,
            productVariantId: product.variants[0].id,
            quantity: 2,
            unitPrice: null,
          },
        ],
        method: "Cash",
        paidOn: null,
        notes: null,
        isDelivered: true,
      }),
    );
  });
});
