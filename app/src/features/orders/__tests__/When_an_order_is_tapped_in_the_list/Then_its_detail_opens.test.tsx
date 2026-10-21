import { fireEvent, screen } from "@testing-library/react-native";
import { listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock, segmentsMock } from "@/testing/expoRouterMock";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const order = buildOrder({ clientFullName: "Ana Pérez" });

describe("When an order is tapped in the list", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([order]);
    segmentsMock.current = ["(tabs)", "settings", "orders"];
  });

  it("Then its detail opens", async () => {
    await renderWithProviders(<OrderListScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("orders.open", { number: order.number, client: "Ana Pérez" }),
      }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.order("settings", order.id));
  });
});
