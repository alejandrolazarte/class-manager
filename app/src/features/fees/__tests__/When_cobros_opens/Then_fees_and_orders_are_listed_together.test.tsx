import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
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
const paidClient = buildClientFee({
  clientId: "paid",
  clientFullName: "Aldana Paz",
  paid: 12000,
  balance: 0,
  status: "Paid",
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
const packOrder = buildOrder({ id: "pack-order", number: 1042, lines: [packLine], total: 80 });

describe("When cobros opens", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([debtor, paidClient]));
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest.mocked(listOrders).mockResolvedValue([packOrder]);
  });

  it("Then fees and orders are listed together", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    expect(await screen.findByText(/n\.º 1042/)).toBeOnTheScreen();
    expect(screen.getByText(debtor.clientFullName)).toBeOnTheScreen();
    expect(screen.getByText(paidClient.clientFullName)).toBeOnTheScreen();
  });
});
