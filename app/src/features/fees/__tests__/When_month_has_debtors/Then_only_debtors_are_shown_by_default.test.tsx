import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

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

describe("When month has debtors", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([debtor, paidClient]));
  });

  it("Then only debtors are shown by default", async () => {
    await renderWithProviders(<MonthlyFeesScreen initialMonth={feeMonth} />);

    expect(await screen.findByText(debtor.clientFullName)).toBeOnTheScreen();
    expect(screen.queryByText(paidClient.clientFullName)).not.toBeOnTheScreen();
  });
});
