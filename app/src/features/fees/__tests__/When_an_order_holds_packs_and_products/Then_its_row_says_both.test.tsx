import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrder, buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

const productOrder = buildOrder({ id: "mixed-order" });
const mixedOrder = buildOrder({
  id: "mixed-order",
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
    ...productOrder.lines,
  ],
});

describe("When an order holds packs and products", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([]));
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest.mocked(listOrders).mockResolvedValue([mixedOrder]);
  });

  it("Then its row says both", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    expect(await screen.findByText(translate("collections.tag.classPack"))).toBeOnTheScreen();
    expect(screen.getByText(translate("collections.tag.product"))).toBeOnTheScreen();
  });
});
