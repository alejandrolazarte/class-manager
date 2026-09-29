import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { confirmOrderPayment, listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const requestedOrder = buildOrder({
  channel: "App",
  status: "Requested",
  awaitsPickup: false,
  method: null,
  paidOn: null,
  clientFullName: "Ana Pérez",
});

describe("When branch confirms an app order", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([requestedOrder]);
    jest.mocked(confirmOrderPayment).mockResolvedValue(buildOrder());
  });

  it("Then the payment is sent", async () => {
    await renderWithProviders(<OrderListScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("fees.methods.Card") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("orders.confirmPayment") }));

    await waitFor(() =>
      expect(confirmOrderPayment).toHaveBeenCalledWith(requestedOrder.id, "Card", true),
    );
  });
});
