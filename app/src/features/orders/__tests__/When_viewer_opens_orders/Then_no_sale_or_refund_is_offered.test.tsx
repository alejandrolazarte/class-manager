import { screen } from "@testing-library/react-native";
import { listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { buildViewer } from "@/testing/memberFactory";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

describe("When viewer opens orders", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([buildOrder()]);
  });

  it("Then no sale or refund is offered", async () => {
    await renderWithProviders(<OrderListScreen />, { member: buildViewer() });

    await screen.findByText(translate("orders.noFamily"));
    expect(screen.queryByRole("button", { name: translate("orders.counterSale.open") })).toBeNull();
    expect(screen.queryByRole("button", { name: translate("orders.refund.open") })).toBeNull();
  });
});
