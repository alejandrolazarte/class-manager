import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrder, buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

const owedClassPackOrder = buildOrder({
  paymentStatus: "Unpaid",
  awaitsPickup: false,
  payments: [],
  paidAmount: 0,
  total: 80,
  balance: 80,
  lines: [
    {
      id: "0192f0c4-0000-7000-8000-00000000d0b1",
      kind: "ClassPack",
      name: "4 clases",
      quantity: 1,
      unitPrice: 80,
      total: 80,
      refundedQuantity: 0,
      refundedAmount: 0,
      classPackPurchaseId: "0192f0c4-0000-7000-8000-00000000d0c1",
    },
  ],
});

describe("When only class packs are owed", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([]));
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest
      .mocked(listOrders)
      .mockImplementation(async (_clientId, filter) =>
        filter === "owing" ? [owedClassPackOrder] : [],
      );
  });

  it("Then the orders view shows everything paid", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    await fireEvent.press(
      await screen.findByRole("tab", { name: translate("collections.orders") }),
    );

    expect(await screen.findByText(translate("collections.allOrdersPaid"))).toBeOnTheScreen();
  });
});
