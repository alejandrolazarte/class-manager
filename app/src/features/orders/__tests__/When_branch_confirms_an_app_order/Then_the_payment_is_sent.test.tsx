import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listOrders, recordOrderPayment } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const requestedOrder = buildOrder({
  channel: "App",
  paymentStatus: "Unpaid",
  lastPaidOn: null,
  paidAmount: 0,
  balance: 24,
  payments: [],
  clientFullName: "Ana Pérez",
});

describe("When branch confirms an app order", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([requestedOrder]);
    jest.mocked(recordOrderPayment).mockResolvedValue(buildOrder());
  });

  it("Then the payment is sent", async () => {
    await renderWithProviders(<OrderListScreen />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.collect") }));

    await fireEvent.press(screen.getByRole("button", { name: translate("fees.methods.Card") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("orders.confirmPayment") }));

    await waitFor(() =>
      expect(recordOrderPayment).toHaveBeenCalledWith(requestedOrder.id, {
        amount: 24,
        method: "Card",
        isDelivered: false,
        isReady: true,
      }),
    );
  });
});
