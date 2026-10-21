import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getOrder, refundOrder } from "@/features/orders/ordersApi";
import { OrderDetailScreen } from "@/features/orders/screens/OrderDetailScreen";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const deliveredOrder = buildOrder({ status: "Delivered", awaitsPickup: false });
const line = deliveredOrder.lines[0];

describe("When branch records a refund", () => {
  beforeEach(() => {
    jest.mocked(getOrder).mockResolvedValue(deliveredOrder);
    jest.mocked(refundOrder).mockResolvedValue(deliveredOrder);
  });

  it("Then nothing is refunded until confirming", async () => {
    await renderWithProviders(<OrderDetailScreen orderId={deliveredOrder.id} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.refund.open") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("orders.refund.confirm") }));

    expect(
      screen.getByText(
        translate("orders.cancelConfirm.refund", {
          amount: formatMoney(line.unitPrice * line.quantity, "ARS"),
          businessName: buildBusiness().name,
        }),
      ),
    ).toBeOnTheScreen();
    expect(refundOrder).not.toHaveBeenCalled();

    const confirmButtons = screen.getAllByRole("button", {
      name: translate("orders.refund.confirm"),
    });
    await fireEvent.press(confirmButtons[confirmButtons.length - 1]);

    await waitFor(() =>
      expect(refundOrder).toHaveBeenCalledWith(deliveredOrder.id, [
        { lineId: line.id, quantity: line.quantity, returnToStock: true },
      ]),
    );
  });
});
