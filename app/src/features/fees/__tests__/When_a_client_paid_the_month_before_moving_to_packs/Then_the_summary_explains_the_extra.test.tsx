import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { formatMoney } from "@/features/fees/money";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { translate } from "@/i18n/translate";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const monthlyClient = buildClientFee({ fee: 12000, paid: 12000, balance: 0, status: "Paid" });
const paidByClassPackClients = 25000;

describe("When a client paid the month before moving to packs", () => {
  beforeEach(() => {
    const monthlyFees = buildMonthlyFees([monthlyClient]);
    jest.mocked(listMonthlyFees).mockResolvedValue({
      ...monthlyFees,
      totalPaid: monthlyFees.totalPaid + paidByClassPackClients,
    });
  });

  it("Then the summary explains the extra", async () => {
    await renderWithProviders(<MonthlyFeesScreen initialMonth={feeMonth} />);

    expect(
      await screen.findByText(
        translate("fees.month.paidByClassPackClients", {
          amount: formatMoney(paidByClassPackClients, "ARS"),
        }),
      ),
    ).toBeOnTheScreen();
  });
});
