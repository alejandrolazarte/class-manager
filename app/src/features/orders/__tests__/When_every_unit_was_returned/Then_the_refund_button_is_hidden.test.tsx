import { screen } from "@testing-library/react-native";
import { listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const pendingOrder = buildOrder();
const returnedOrder = buildOrder({
  status: "Delivered",
  awaitsPickup: false,
  refundedAmount: 24,
  lines: pendingOrder.lines.map((line) => ({
    ...line,
    refundedQuantity: line.quantity,
    refundedAmount: line.total,
  })),
});

describe("When every unit was returned", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([returnedOrder]);
  });

  it("Then the refund button is hidden", async () => {
    await renderWithProviders(<OrderListScreen />);

    expect(await screen.findByText("Gorro de natación", { exact: false })).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: translate("orders.refund.open") })).toBeNull();
  });
});
