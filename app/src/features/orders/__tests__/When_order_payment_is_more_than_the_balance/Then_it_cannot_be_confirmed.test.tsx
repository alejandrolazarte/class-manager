import { fireEvent, screen } from "@testing-library/react-native";
import { listOrders, recordOrderPayment } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

describe("When order payment is more than the balance", () => {
  beforeEach(() => {
    jest
      .mocked(listOrders)
      .mockResolvedValue([
        buildOrder({ paymentStatus: "Unpaid", paidAmount: 0, balance: 24, payments: [] }),
      ]);
  });

  it("Then it cannot be confirmed", async () => {
    await renderWithProviders(<OrderListScreen />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.collect") }));

    await fireEvent.changeText(screen.getByLabelText(translate("orders.payment.amount")), "30");
    await fireEvent.press(screen.getByRole("button", { name: translate("orders.confirmPayment") }));

    expect(
      screen.getByText(
        translate("orders.payment.overBalance", { balance: formatMoney(24, "ARS") }),
      ),
    ).toBeOnTheScreen();
    expect(recordOrderPayment).not.toHaveBeenCalled();
  });
});
