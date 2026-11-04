import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { formatMoney } from "@/formatting/money";
import { buildBusiness } from "@/testing/businessFactory";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrderSummary } from "@/testing/productFactory";
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
const money = (amount: number) => formatMoney(amount, buildBusiness().currencyCode);

describe("When month has fees and orders", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([debtor, paidClient]));
    jest.mocked(getOrderSummary).mockResolvedValue(
      buildOrderSummary({
        collected: 300,
        unpaid: 100,
        classPacksCollected: 200,
        productsCollected: 100,
      }),
    );
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  it("Then the collected total adds them up", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    expect(await screen.findByText(money(12000))).toBeOnTheScreen();
    expect(screen.getByText(money(200))).toBeOnTheScreen();
    expect(screen.getByText(money(12200))).toBeOnTheScreen();
    expect(screen.getByText(money(12300))).toBeOnTheScreen();
  });
});
