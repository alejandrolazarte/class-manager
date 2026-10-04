import { screen } from "@testing-library/react-native";
import { listOrders } from "@/features/orders/ordersApi";
import { NotifiedOrdersScreen } from "@/features/orders/screens/NotifiedOrdersScreen";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { buildCurrentMember } from "@/testing/memberFactory";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const ordersOnlyMember = buildCurrentMember({
  branchRole: "Coach",
  isBrandOwner: false,
  permissions: ["business.view", "orders.view.all"],
});

describe("When order notification is opened without payments", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([buildOrder()]);
  });

  it("Then orders stay in today", async () => {
    await renderWithProviders(<NotifiedOrdersScreen />, { member: ordersOnlyMember });

    await screen.findByText(translate("orders.noClient"));
    expect(routerMock.replace).not.toHaveBeenCalled();
  });
});
