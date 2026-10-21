import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { cancelOrder, listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const partlyPaidOrder = buildOrder({
  paymentStatus: "Partial",
  paidAmount: 10,
  balance: 14,
  payments: [
    { id: "payment-1", amount: 10, method: "Cash", paidOn: "2026-09-29", recordedByUserId: null },
  ],
});

describe("When branch cancels an order paid in part", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([partlyPaidOrder]);
    jest.mocked(cancelOrder).mockResolvedValue(buildOrder({ status: "Cancelled" }));
  });

  it("Then it is cancelled after confirming", async () => {
    await renderWithProviders(<OrderListScreen />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.cancel") }));

    expect(
      screen.getByText(
        translate("orders.cancelConfirm.refund", {
          amount: formatMoney(10, "ARS"),
          businessName: buildBusiness().name,
        }),
        { exact: false },
      ),
    ).toBeOnTheScreen();
    expect(cancelOrder).not.toHaveBeenCalled();

    const cancelButtons = screen.getAllByRole("button", { name: translate("orders.cancel") });
    await fireEvent.press(cancelButtons[cancelButtons.length - 1]);

    await waitFor(() => expect(cancelOrder).toHaveBeenCalledWith(partlyPaidOrder.id));
  });
});
