import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listOrders, recordOrderPayment } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
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

describe("When order is paid in part", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([partlyPaidOrder]);
    jest.mocked(recordOrderPayment).mockResolvedValue(buildOrder());
  });

  it("Then the typed amount is sent", async () => {
    await renderWithProviders(<OrderListScreen />);
    expect(
      await screen.findByText(
        translate("orders.paidAndBalance", {
          paid: formatMoney(10, "ARS"),
          balance: formatMoney(14, "ARS"),
        }),
      ),
    ).toBeOnTheScreen();
    await fireEvent.press(screen.getByRole("button", { name: translate("orders.collect") }));

    await fireEvent.changeText(screen.getByLabelText(translate("orders.payment.amount")), "4");
    await fireEvent.press(screen.getByRole("button", { name: translate("orders.confirmPayment") }));

    await waitFor(() =>
      expect(recordOrderPayment).toHaveBeenCalledWith(
        partlyPaidOrder.id,
        expect.objectContaining({ amount: 4 }),
      ),
    );
  });
});
