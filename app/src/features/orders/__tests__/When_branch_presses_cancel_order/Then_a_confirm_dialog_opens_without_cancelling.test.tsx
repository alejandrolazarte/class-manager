import { fireEvent, screen } from "@testing-library/react-native";
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

describe("When branch presses cancel order", () => {
  beforeEach(() => {
    jest.mocked(getOrder).mockResolvedValue(requestedOrder);
  });

  it("Then a confirm dialog opens without cancelling", async () => {
    await renderWithProviders(<OrderDetailScreen orderId={requestedOrder.id} />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.cancel") }));

    expect(
      screen.getByText(
        translate("orders.cancelQuestionWithClient", {
          number: requestedOrder.number,
          client: "Ana Pérez",
        }),
      ),
    ).toBeOnTheScreen();
    expect(cancelOrder).not.toHaveBeenCalled();
  });
});
