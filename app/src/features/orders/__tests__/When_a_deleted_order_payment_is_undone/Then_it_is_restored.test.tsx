import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { deleteOrderPayment, listOrders, restoreOrderPayment } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const paidOrder = buildOrder();
const payment = paidOrder.payments[0];
const deleteLabel = translate("orders.payments.deleteOf", {
  amount: formatMoney(payment.amount, "ARS"),
});

describe("When a deleted order payment is undone", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([paidOrder]);
    jest.mocked(deleteOrderPayment).mockResolvedValue(buildOrder());
    jest.mocked(restoreOrderPayment).mockResolvedValue(buildOrder());
  });

  it("Then it is restored", async () => {
    await renderWithProviders(<OrderListScreen />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.payments.open") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: deleteLabel }));

    await fireEvent.press(await screen.findByRole("button", { name: translate("common.undo") }));

    await waitFor(() => expect(restoreOrderPayment).toHaveBeenCalledWith(paidOrder.id, payment.id));
  });
});
