import { screen } from "@testing-library/react-native";
import { listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

describe("When an owing order is listed", () => {
  beforeEach(() => {
    jest
      .mocked(listOrders)
      .mockResolvedValue([
        buildOrder({ paymentStatus: "Partial", paidAmount: 10, balance: 14, payments: [] }),
      ]);
  });

  it("Then only collect is offered", async () => {
    await renderWithProviders(<OrderListScreen />);

    expect(
      await screen.findByRole("button", { name: translate("orders.collect") }),
    ).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: translate("orders.markDelivered") })).toBeNull();
    expect(screen.queryByRole("button", { name: translate("orders.cancel") })).toBeNull();
  });
});
