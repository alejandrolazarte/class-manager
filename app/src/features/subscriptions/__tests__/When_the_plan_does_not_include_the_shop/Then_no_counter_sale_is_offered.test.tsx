import { screen } from "@testing-library/react-native";
import { listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/orders/ordersApi");

const member = buildCurrentMember({
  subscription: buildSubscription({ features: [{ code: featureCodes.classPacks, limit: null }] }),
});

describe("When the plan does not include the shop", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([buildOrder()]);
  });

  it("Then no counter sale is offered", async () => {
    await renderWithProviders(<OrderListScreen />, { member });

    await screen.findByText(translate("orders.noClient"));
    expect(screen.queryByRole("button", { name: translate("orders.counterSale.open") })).toBeNull();
  });
});
