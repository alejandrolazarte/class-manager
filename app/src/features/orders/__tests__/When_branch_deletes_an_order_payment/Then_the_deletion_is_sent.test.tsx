import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { deleteOrderPayment, getOrder, restoreOrderPayment } from "@/features/orders/ordersApi";
import { OrderDetailScreen } from "@/features/orders/screens/OrderDetailScreen";
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
    jest.mocked(getOrder).mockResolvedValue(paidOrder);
    jest.mocked(deleteOrderPayment).mockResolvedValue(buildOrder());
  });

  it("Then the deletion is sent", async () => {
    await renderWithProviders(<OrderDetailScreen orderId={paidOrder.id} />);

    await fireEvent.press(await screen.findByRole("button", { name: deleteLabel }));

    await waitFor(() => expect(deleteOrderPayment).toHaveBeenCalledWith(paidOrder.id, payment.id));
    expect(restoreOrderPayment).not.toHaveBeenCalled();
  });
});
