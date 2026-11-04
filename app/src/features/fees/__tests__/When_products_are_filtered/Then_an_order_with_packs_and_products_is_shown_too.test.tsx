import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrder, buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

const debtor = buildClientFee({
  clientId: "debtor",
  clientFullName: "Zoe Ruiz",
  paid: 0,
  balance: 12000,
  status: "Unpaid",
});
const packLine = {
  id: "0192f0c4-0000-7000-8000-00000000d0b1",
  kind: "ClassPack" as const,
  name: "4 clases",
  quantity: 1,
  unitPrice: 80,
  total: 80,
  refundedQuantity: 0,
  refundedAmount: 0,
  classPackPurchaseId: "0192f0c4-0000-7000-8000-00000000d0c1",
};
const productOrder = buildOrder({ id: "product-order", number: 1041 });
const packOrder = buildOrder({ id: "pack-order", number: 1042, lines: [packLine], total: 80 });
const mixedOrder = buildOrder({
  id: "mixed-order",
  number: 1043,
  lines: [packLine, ...productOrder.lines],
  total: 104,
});

describe("When products are filtered", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([debtor]));
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest.mocked(listOrders).mockResolvedValue([productOrder, packOrder, mixedOrder]);
  });

  it("Then an order with packs and products is shown too", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);
    await screen.findByText(/n\.º 1042/);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("collections.kind.products") }),
    );

    expect(screen.getByText(/n\.º 1043/)).toBeOnTheScreen();
    expect(screen.getByText(/n\.º 1041/)).toBeOnTheScreen();
    expect(screen.queryByText(/n\.º 1042/)).not.toBeOnTheScreen();
  });
});
