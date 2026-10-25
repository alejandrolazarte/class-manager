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

const packOrder = buildOrder({
  channel: "App",
  paymentStatus: "Unpaid",
  awaitsPickup: false,
  paidAmount: 0,
  balance: 25,
  total: 25,
  payments: [],
  lines: [
    {
      id: "0192f0c4-0000-7000-8000-00000000d0b1",
      kind: "ClassPack",
      name: "4 clases",
      quantity: 1,
      unitPrice: 25,
      total: 25,
      refundedQuantity: 0,
      refundedAmount: 0,
      classPackPurchaseId: null,
    },
  ],
});
const member = buildCurrentMember({
  subscription: buildSubscription({ features: [{ code: featureCodes.classPacks, limit: null }] }),
});

describe("When the plan does not include the shop", () => {
  beforeEach(() => {
    jest.mocked(getOrder).mockResolvedValue(packOrder);
  });

  it("Then a pack order can be collected and handed over", async () => {
    await renderWithProviders(<OrderDetailScreen orderId={packOrder.id} />, { member });

    expect(
      await screen.findByRole("button", { name: translate("orders.markDelivered") }),
    ).toBeOnTheScreen();
    expect(screen.getByRole("button", { name: translate("orders.collect") })).toBeOnTheScreen();
  });
});
