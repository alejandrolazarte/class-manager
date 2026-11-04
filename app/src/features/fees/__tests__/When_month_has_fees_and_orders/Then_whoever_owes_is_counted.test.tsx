import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { formatMoney } from "@/formatting/money";
import { translateCount } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
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
const money = (amount: number) => formatMoney(amount, buildBusiness().currencyCode);

const owingOrder = buildOrder({
  id: "owing-order",
  paymentStatus: "Unpaid",
  payments: [],
  paidAmount: 0,
  balance: 24,
});

describe("When month has fees and orders", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([debtor, paidClient]));
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary({ unpaid: 24 }));
    jest
      .mocked(listOrders)
      .mockImplementation(async (_clientId, filter) => (filter === "owing" ? [owingOrder] : []));
  });

  it("Then whoever owes is counted", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    expect(
      await screen.findByText(translateCount("collections.owing", 2, { amount: money(12024) })),
    ).toBeOnTheScreen();
  });
});
