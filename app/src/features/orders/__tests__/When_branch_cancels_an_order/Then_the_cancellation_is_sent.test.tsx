import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { cancelOrder, getOrder } from "@/features/orders/ordersApi";
import { OrderDetailScreen } from "@/features/orders/screens/OrderDetailScreen";
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

describe("When branch cancels an order", () => {
  beforeEach(() => {
    jest.mocked(getOrder).mockResolvedValue(requestedOrder);
    jest.mocked(cancelOrder).mockResolvedValue(buildOrder({ status: "Cancelled" }));
  });

  it("Then the cancellation is sent", async () => {
    await renderWithProviders(<OrderDetailScreen orderId={requestedOrder.id} />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.cancel") }));
    await fireEvent.press(
      screen.getAllByRole("button", { name: translate("orders.cancel") }).at(-1)!,
    );

    await waitFor(() => expect(cancelOrder).toHaveBeenCalledWith(requestedOrder.id));
  });
});
