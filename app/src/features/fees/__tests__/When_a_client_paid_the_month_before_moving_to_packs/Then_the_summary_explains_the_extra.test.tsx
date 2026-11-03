import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { formatMoney } from "@/formatting/money";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { translate } from "@/i18n/translate";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

const monthlyClient = buildClientFee({ fee: 12000, paid: 12000, balance: 0, status: "Paid" });
const paidByClassPackClients = 25000;

describe("When a client paid the month before moving to packs", () => {
  beforeEach(() => {
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  beforeEach(() => {
    const monthlyFees = buildMonthlyFees([monthlyClient]);
    jest.mocked(listMonthlyFees).mockResolvedValue({
      ...monthlyFees,
      totalPaid: monthlyFees.totalPaid + paidByClassPackClients,
    });
  });

  it("Then the summary explains the extra", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    expect(
      await screen.findByText(
        translate("fees.month.paidByClassPackClients", {
          amount: formatMoney(paidByClassPackClients, "ARS"),
        }),
      ),
    ).toBeOnTheScreen();
  });
});
