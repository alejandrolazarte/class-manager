import { fireEvent, screen } from "@testing-library/react-native";
import { listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const orderWithoutClient = buildOrder({ clientFullName: null });

describe("When branch presses cancel on an order without client", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([orderWithoutClient]);
  });

  it("Then the question has no client name", async () => {
    await renderWithProviders(<OrderListScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.cancel") }));

    expect(
      screen.getByText(translate("orders.cancelQuestion", { number: orderWithoutClient.number })),
    ).toBeOnTheScreen();
  });
});
