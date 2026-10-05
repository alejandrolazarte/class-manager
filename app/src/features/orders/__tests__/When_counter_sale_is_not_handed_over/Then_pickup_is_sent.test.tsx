import { fireEvent, screen, waitFor, within } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { createCounterSale } from "@/features/orders/ordersApi";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { translate } from "@/i18n/translate";
import { buildOrder, buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/products/productsApi");
jest.mock("@/features/classPacks/classPacksApi");

describe("When counter sale is not handed over", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([buildProduct()]);
    jest.mocked(listClassPacks).mockResolvedValue([]);
    jest.mocked(createCounterSale).mockResolvedValue(buildOrder());
  });

  it("Then pickup is sent", async () => {
    const product = buildProduct();
    await renderWithProviders(<CounterSaleScreen />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.counterSale.addItems") }),
    );
    const catalog = within(screen.getByTestId("counter-sale-catalog"));
    await fireEvent.press(
      await catalog.findByRole("button", {
        name: translate("orders.counterSale.add", { name: product.name }),
      }),
    );
    await fireEvent.press(catalog.getByRole("button", { name: translate("common.done") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.continue") }));
    await fireEvent.press(
      screen.getByRole("switch", { name: translate("orders.counterSale.handedOver") }),
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("orders.counterSale.charge") }),
    );

    await waitFor(() =>
      expect(createCounterSale).toHaveBeenCalledWith(
        expect.objectContaining({ isDelivered: false, delivery: "Pickup" }),
      ),
    );
  });
});
