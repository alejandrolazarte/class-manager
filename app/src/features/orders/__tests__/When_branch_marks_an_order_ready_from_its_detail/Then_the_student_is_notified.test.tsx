import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getOrder, markOrderReady } from "@/features/orders/ordersApi";
import { OrderDetailScreen } from "@/features/orders/screens/OrderDetailScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const paidOrder = buildOrder();

describe("When branch marks an order ready from its detail", () => {
  beforeEach(() => {
    jest.mocked(getOrder).mockResolvedValue(paidOrder);
    jest.mocked(markOrderReady).mockResolvedValue(paidOrder);
  });

  it("Then the student is notified", async () => {
    await renderWithProviders(<OrderDetailScreen orderId={paidOrder.id} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.markReady") }),
    );

    await waitFor(() => expect(markOrderReady).toHaveBeenCalledWith(paidOrder.id));
    expect(await screen.findByText(translate("orders.readyNotified"))).toBeOnTheScreen();
  });
});
