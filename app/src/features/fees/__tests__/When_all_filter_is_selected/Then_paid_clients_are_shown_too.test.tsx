import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { translate } from "@/i18n/translate";
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

describe("When all filter is selected", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([debtor, paidClient]));
  });

  it("Then paid clients are shown too", async () => {
    await renderWithProviders(<MonthlyFeesScreen initialMonth={feeMonth} />);
    await screen.findByText(debtor.clientFullName);

    await fireEvent.press(screen.getByRole("button", { name: translate("fees.month.filterAll") }));

    expect(screen.getByText(paidClient.clientFullName)).toBeOnTheScreen();
  });
});
