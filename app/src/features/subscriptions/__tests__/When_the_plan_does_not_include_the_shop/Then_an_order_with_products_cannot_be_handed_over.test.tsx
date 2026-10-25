import { screen } from "@testing-library/react-native";
import { getOrder } from "@/features/orders/ordersApi";
import { OrderDetailScreen } from "@/features/orders/screens/OrderDetailScreen";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/orders/ordersApi");

const productOrder = buildOrder();
const member = buildCurrentMember({
  subscription: buildSubscription({ features: [{ code: featureCodes.classPacks, limit: null }] }),
});

describe("When the plan does not include the shop", () => {
  beforeEach(() => {
    jest.mocked(getOrder).mockResolvedValue(productOrder);
  });

  it("Then an order with products cannot be handed over", async () => {
    await renderWithProviders(<OrderDetailScreen orderId={productOrder.id} />, { member });

    await screen.findByText(translate("orders.detail.delivery"));
    expect(screen.queryByRole("button", { name: translate("orders.markDelivered") })).toBeNull();
  });
});
