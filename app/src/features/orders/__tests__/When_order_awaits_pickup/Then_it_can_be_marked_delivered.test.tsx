import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listOrders, markOrderDelivered } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

describe("When order awaits pickup", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([buildOrder()]);
    jest
      .mocked(markOrderDelivered)
      .mockResolvedValue(buildOrder({ status: "Delivered", awaitsPickup: false }));
  });

  it("Then it can be marked delivered", async () => {
    await renderWithProviders(<OrderListScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.markDelivered") }),
    );

    await waitFor(() => expect(markOrderDelivered).toHaveBeenCalledWith(buildOrder().id));
  });
});
