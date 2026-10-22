import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getOrder, markOrderDelivered } from "@/features/orders/ordersApi";
import { OrderDetailScreen } from "@/features/orders/screens/OrderDetailScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const unpaidOrder = buildOrder({
  paymentStatus: "Unpaid",
  paidAmount: 0,
  balance: 24,
  payments: [],
});

describe("When unpaid order is handed over", () => {
  beforeEach(() => {
    jest.mocked(getOrder).mockResolvedValue(unpaidOrder);
    jest.mocked(markOrderDelivered).mockResolvedValue(buildOrder({ status: "Delivered" }));
  });

  it("Then delivery is sent", async () => {
    await renderWithProviders(<OrderDetailScreen orderId={unpaidOrder.id} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.markDelivered") }),
    );

    await waitFor(() => expect(markOrderDelivered).toHaveBeenCalledWith(unpaidOrder.id));
  });
});
