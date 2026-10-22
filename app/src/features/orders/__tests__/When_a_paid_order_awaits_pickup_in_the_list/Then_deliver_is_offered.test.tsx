import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listOrders, markOrderDelivered } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const paidOrder = buildOrder();

describe("When a paid order awaits pickup in the list", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([paidOrder]);
    jest.mocked(markOrderDelivered).mockResolvedValue(buildOrder({ status: "Delivered" }));
  });

  it("Then deliver is offered", async () => {
    await renderWithProviders(<OrderListScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.markDelivered") }),
    );

    await waitFor(() => expect(markOrderDelivered).toHaveBeenCalledWith(paidOrder.id));
  });
});
