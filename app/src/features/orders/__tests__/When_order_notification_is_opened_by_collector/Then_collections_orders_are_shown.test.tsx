import { waitFor } from "@testing-library/react-native";
import { listOrders } from "@/features/orders/ordersApi";
import { NotifiedOrdersScreen } from "@/features/orders/screens/NotifiedOrdersScreen";
import { routes } from "@/navigation/routes";
import { navigationMock, routerMock } from "@/testing/expoRouterMock";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

describe("When order notification is opened by collector", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  it("Then collections orders are shown", async () => {
    await renderWithProviders(<NotifiedOrdersScreen />, { member: buildCurrentMember() });

    await waitFor(() =>
      expect(routerMock.navigate).toHaveBeenCalledWith(routes.collections("orders")),
    );
    expect(navigationMock.reset).toHaveBeenCalledWith({ index: 0, routes: [{ name: "index" }] });
    expect(routerMock.replace).not.toHaveBeenCalled();
  });
});
