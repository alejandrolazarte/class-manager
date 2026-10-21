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

describe("When branch deletes an order payment", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([paidOrder]);
    jest.mocked(deleteOrderPayment).mockResolvedValue(buildOrder());
  });

  it("Then the deletion is sent", async () => {
    await renderWithProviders(<OrderListScreen />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.payments.open") }),
    );

    await fireEvent.press(screen.getByRole("button", { name: deleteLabel }));

    await waitFor(() => expect(deleteOrderPayment).toHaveBeenCalledWith(paidOrder.id, payment.id));
    expect(restoreOrderPayment).not.toHaveBeenCalled();
  });
});
